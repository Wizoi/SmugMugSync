using SmugMugCore.Net.Core20;
using SmugMugCore.Net.Data20;
using SmugMugCore.Net.Service20;
using SmugMugCoreSync.Configuration;
using SmugMugCoreSync.Data;
using System.Diagnostics;

namespace SmugMugCoreSync.Repositories
{
    public class TargetAlbumRepository
    {
        private readonly FolderSyncPathsConfig? _folderSyncPathsConfig;
        private readonly SmugMugCore.Net.Core20.SmugMugCore? _smCore;
        private readonly Dictionary<string, AlbumDetail> _targetAlbums = [];

        public TargetAlbumRepository() : this(null, null) { }
        
        public TargetAlbumRepository(SmugMugCore.Net.Core20.SmugMugCore? core, FolderSyncPathsConfig? folderConfig)
        {
            _smCore = core;
            _folderSyncPathsConfig = folderConfig;
        }

        public int TargetAlbumCount()
        {
            return _targetAlbums.Count;
        }

        public async Task<bool> PopulateTargetAlbums()
        {
            Trace.Write("Populating Target Albums...");
            bool result = await LoadRemoteAlbums();
            Trace.WriteLine($"{_targetAlbums.Count} found.");

            return result;
        }

        public void VerifyLinkedFolders(RuntimeFlagsConfig runtimeFlags, SourceFolderRepository sourceFolders)
        {
            foreach (var f in sourceFolders.RetrieveLinkedFolders())
            {
                if (!_targetAlbums.ContainsKey(f.AlbumKey))
                {
                    // Folder is marked as linked to an album which does not  match
                    f.UnlinkFromAlbum();
                    sourceFolders.RemoveLinkedFolder(f);
                }
            }
        }

        public async Task<bool> SyncNewFolders(RuntimeFlagsConfig runtimeFlags, SourceFolderRepository sourceFolders)
        {
            var newFolders = sourceFolders.RetrieveUnlinkedFolders();

            foreach (var f in newFolders)
            {
                var alb = new SmugMugCore.Net.Data20.AlbumDetail()
                {
                    SortDirection = "Descending",
                    SortMethod = "Date Taken",
                    LargestSize = "Original",
                    EXIF = true,
                    Comments = true,
                    CanRank = true,
                    CanShare = true,
                    Privacy = "Public",
                    Geography = true,
                    Name = f.FolderName
                };

                switch (runtimeFlags.TargetCreate)
                {
                    case OperationLevel.Normal:
                        Trace.WriteLine($"    > Creating Album: {alb.Name}");
                        var stubNewAlbum = await _smCore.AlbumService.CreateAlbum(alb);
                        var createdAlbum = await _smCore.AlbumService.GetAlbumDetail(stubNewAlbum.AlbumKey);

                        // Link the folder to the album, and add the new album to the internal objects
                        f.LinkToAlbum(albumKey: stubNewAlbum.AlbumKey);
                        _targetAlbums.Add(createdAlbum.AlbumKey, createdAlbum);
                        sourceFolders.AddNewLinkedFolder(f);

                        break;
                    case OperationLevel.NoneLog:
                        Trace.WriteLine($"    >? Album Create Suppressed: {alb.Name}");
                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        public async Task<bool> SyncExistingFolders(RuntimeFlagsConfig runtimeFlags, SourceFolderRepository sourceFolders)
        {
            var existingFolders = sourceFolders.RetrieveLinkedFolders();
            var keyHashList = existingFolders.ToLookup(x => x.AlbumKey); 

            foreach (var albumKey in _targetAlbums.Keys)
            {
                if (!keyHashList.Contains(albumKey))
                {
                    switch (runtimeFlags.TargetDelete)
                    {
                        case OperationLevel.Normal:
                            Trace.WriteLine($"    > Deleting Album: {albumKey} / {_targetAlbums[albumKey].Name}");
                            if (await _smCore.AlbumService.DeleteAlbum(_targetAlbums[albumKey]))
                            {
                                _targetAlbums.Remove(albumKey);
                            }
                            break;
                        case OperationLevel.NoneLog:
                            Trace.WriteLine($"    >? Album Delete Suppressed: {albumKey} / {_targetAlbums[albumKey].Name}");
                            break;
                        default:
                            break;
                    }
                }
            }

            return true;
        }

        public async Task<RuntimeFolderStats> SyncFolderFiles(RuntimeFlagsConfig runtimeFlags, SourceFolderRepository sourceFolders)
        {
            var uploadThrottler = new SemaphoreSlim(runtimeFlags.ImageUploadThrottle);
            var runtimeStats = new RuntimeFolderStats();

            var albumSortedList = _targetAlbums.Values.OrderBy(x => x.Name);
            foreach (var targetAlbum in albumSortedList)
            {
                runtimeStats.ProcessedFolders++;
                
                Trace.WriteLine($"  Album: {targetAlbum.Name}");
                var sourceFolder = sourceFolders.RetrieveLinkedFolderByKey(targetAlbum.AlbumKey);
                if (sourceFolder == null)
                {
                    runtimeStats.SkippedFolders++;
                    Trace.WriteLine($"    > SKIP - Source folder is not found / linked to this album");
                    continue;
                }

                //
                // Load the source files and album images for the current linked album 
                //
                var runtimeFileStats = runtimeStats.StartNewFolderStats();
                runtimeFileStats.FolderName = targetAlbum.Name ?? String.Empty;
                
                var sourceFiles = sourceFolders.LoadFolderMediaFiles(sourceFolder);

                AlbumImageDetail[] targetFiles = await _smCore.AlbumImageService.GetAlbumImageListShort(albumKey: targetAlbum.AlbumKey);

                //
                // Build a lookup for source files, by a cleaned up filename for indexing
                //
                Dictionary<string, SourceMediaData> sourceFileNameLookup = [];
                Dictionary<string, AlbumImageDetail> targetFileNameLookup = [];

                string[] deleteFilenames;
                string[] newFilenames;
                string[] existingFilenames;
                AlbumImageDetail[] dupeTargetImages;

                foreach (var file in sourceFiles)
                {
                    sourceFileNameLookup.Add(file.FileNameBase.ToUpper(), file);
                }
                List<AlbumImageDetail> dupeFileNameList = [];
                foreach (var file in targetFiles)
                {
                    if (targetFileNameLookup.ContainsKey(file.FileNameBase.ToUpper()))
                        dupeFileNameList.Add(file);
                    else
                        targetFileNameLookup.Add(file.FileNameBase.ToUpper(), file);
                }

                //
                // Do the key magic to determine deleted new, existing and dupe entries
                //
                deleteFilenames = targetFileNameLookup.Keys.Except(sourceFileNameLookup.Keys).ToArray();
                newFilenames = sourceFileNameLookup.Keys.Except(targetFileNameLookup.Keys).ToArray();
                existingFilenames = targetFileNameLookup.Keys.Intersect(sourceFileNameLookup.Keys).ToArray();
                dupeTargetImages = dupeFileNameList.ToArray();

                // Delete old files
                foreach (var targetName in deleteFilenames)
                {
                    var targetImage = targetFileNameLookup[targetName];
                    switch (runtimeFlags.TargetDelete)
                    {
                        case OperationLevel.Normal:
                            Trace.WriteLine($"    > Deleting Remote Media: {targetImage.FileName}");
                            _ = await _smCore.AlbumImageService.DeleteImage(targetImage.Uris.Image.Uri);
                            runtimeFileStats.DeletedFiles++;
                            break;
                        case OperationLevel.NoneLog:
                            Trace.WriteLine($"    >? Delete Suppressed: {targetImage.FileName}");
                            break;
                        default:
                            break;
                    }
                    targetImage.IsDeleted = true;
                }

                // Delete Duplicate files
                foreach (var toDeleteImage in dupeTargetImages)
                {
                    switch (runtimeFlags.TargetDelete)
                    {
                        case OperationLevel.Normal:
                            Trace.WriteLine("    > Deleting Duplicate Media: " + toDeleteImage.Title);
                            _ = await _smCore.AlbumImageService.DeleteImage(toDeleteImage.Uris.Image.Uri);
                            runtimeFileStats.DuplicateFiles++;
                            break;
                        case OperationLevel.NoneLog:
                            Trace.WriteLine("    >? Delete of Dupe Suppressed: " + toDeleteImage.Title);
                            break;
                        default:
                            break;
                    }
                    toDeleteImage.IsDeleted = true;
                }

                // Add new files
                var taskListNew = new List<Task<bool>>();
                foreach (var name in newFilenames)
                {
                    var sourceImage = sourceFileNameLookup[name];
                    taskListNew.Add(ProcessNewRemoteMedia(runtimeFlags, sourceImage, targetAlbum, uploadThrottler));    
                }

                // Check existing files to see if they need to be updated
                var taskListExisting = new List<Task<bool>>();
                foreach (var name in existingFilenames)
                {
                    var sourceImage = sourceFileNameLookup[name];
                    var targetImage = targetFileNameLookup[name];

                    if (!targetImage.IsDeleted)
                    {
                        taskListExisting.Add(ProcessExistingRemoteMedia(runtimeFlags, sourceImage, targetAlbum, targetImage, uploadThrottler));
                    }
                }
                Task.WaitAll(taskListNew.Concat(taskListExisting).ToArray());

                foreach (var t in taskListNew)
                    if (t.Result) runtimeFileStats.AddedFiles++;

                foreach (var t in taskListExisting)
                    if (t.Result) runtimeFileStats.SyncedFiles++;

            }

            return runtimeStats;
        }

        public async virtual Task<bool> ProcessExistingRemoteMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, AlbumDetail targetAlbum, AlbumImageDetail targetImage, SemaphoreSlim uploadThrottler)
        {
            var reDownloadImage = false;

            // Load the metadata for the existing source image
            ContentMetadataService metaSvc = _smCore.ContentMetadataService;
            metaSvc.FileSystem = sourceImage.FileSystem;
            var sourceMetadata = await metaSvc.DiscoverMetadata(sourceImage.FullFileName);

            await uploadThrottler.WaitAsync();
            try
            {

                // Scenario 1: FileLength = 0; Redownload
                if (!reDownloadImage && (sourceImage.FileLength == 0))
                {
                    Trace.WriteLine("    > ERROR / REDOWNLOAD - 0 Byte File Found on source: " + sourceImage.FileName);
                    reDownloadImage = true;
                }

                // Scenario 2: FileLength is different (targetUpdate)
                if (!reDownloadImage && sourceImage.IsImageUpdatable())
                {
                    if ((sourceImage.FileLength != targetImage.ArchivedSize) || runtimeFlags.ForceRefresh)
                    {
                        // Will not redownload a video based on the size being different, as it will be a 
                        // lower quality. And Cannot re-upload a video 2x as SmugMug disallows refreshing, so skipping.
                        if (!sourceMetadata.IsVideo)
                        {
                            reDownloadImage = await IsRedownloadNeeded(_smCore, sourceImage, targetImage);

                            if (!reDownloadImage)
                            {
                                _ = await RefreshRemoteMedia(runtimeFlags, sourceImage, sourceMetadata, targetAlbum, targetImage);
                            }
                        }
                    }
                    else
                    {
                        _ = await UpdateMetadata(runtimeFlags, sourceMetadata, targetImage);
                        reDownloadImage = false;
                    }
                }

                // If the MD5 Image is not matching, but should, we redownload the image with a SmugMug in the filename 
                // to provide the option of viewing the different image (and possibly overwrite to have it match)
                if (reDownloadImage)
                {
                    _ = await RedownloadMedia(runtimeFlags, sourceImage, sourceMetadata, targetImage);
                }
            }
            finally
            {
                uploadThrottler.Release();
            }

            return true;
        } 

        public virtual async Task<bool> ProcessNewRemoteMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, AlbumDetail targetAlbum, SemaphoreSlim uploadThrottler)
        {
            bool continueToAdd = true;

            ContentMetadataService metaSvc = _smCore.ContentMetadataService;
            metaSvc.FileSystem = sourceImage.FileSystem;
            FileMetaContent sourceMetadata = await metaSvc.DiscoverMetadata(sourceImage.FullFileName);

            if (sourceMetadata.IsVideo)
            {
                if (sourceMetadata.VideoLength.TotalMinutes > 20)
                {
                    Trace.WriteLine("    > SKIP (Video too long, > 20 minutes): " + sourceImage.FileName);
                    continueToAdd = false;
                }
                
                if (sourceMetadata.FileInfo?.Length > 2000000000)
                {
                    Trace.WriteLine("    > SKIP (Video size too large, > 2.0 gigs): " + sourceImage.FileName);
                    continueToAdd = false;
                }
                
                if (!runtimeFlags.IncludeVideos)
                {
                    Trace.WriteLine("    > SKIP (Photos Only Enabled): " + sourceImage.FileName);
                    continueToAdd = false;
                }
            }

            if (continueToAdd)
            {
                switch (runtimeFlags.TargetCreate)
                {
                    case OperationLevel.Normal:
                        await uploadThrottler.WaitAsync();
                        try
                        {
                            Trace.WriteLine("    > Adding " + sourceImage.FileName);
                            _ = await UploadNewMedia(_smCore, targetAlbum, sourceMetadata);
                        }
                        finally
                        {
                            uploadThrottler.Release();
                        }

                        return true;
                    case OperationLevel.NoneLog:
                        Trace.WriteLine("    >? Adding Suppressed: " + sourceImage.FileName);
                        break;
                    default:
                        break;
                }
            }

            return false;
        }

        public virtual async Task<bool> RefreshRemoteMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, FileMetaContent sourceMetadata, AlbumDetail targetAlbum, AlbumImageDetail targetImage)
        {
            if (sourceMetadata.IsVideo)
            {
                // Per validation with SmugMug, and integration tests, we cannot update a video
                // already uploaded. Would have to create+publish a whole new video (which I will not do automatically)
                Trace.WriteLine("    > SKIP Update not supported for Videos: " + sourceImage.FileName);
                return false;
            }

            sourceMetadata.MD5Checksum = await sourceImage.LoadMd5Checksum();
            if ((sourceMetadata.MD5Checksum != targetImage.ArchivedMD5))
            {
                switch (runtimeFlags.TargetUpdate)
                {
                    case OperationLevel.Normal:
                        Trace.WriteLine("    > Updating Remote Media: " + sourceImage.FileName);
                        _ = await UploadMedia(_smCore, targetAlbum, targetImage, sourceMetadata);
                        return true;
                    case OperationLevel.NoneLog:
                        Trace.WriteLine("    >? Update Suppressed: " + sourceImage.FileName);
                        break;
                    default:
                        break;
                }
            }

            return false;
        }

        public virtual  async Task<bool> RedownloadMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, FileMetaContent sourceMetadata, AlbumImageDetail targetImage)
        {
            bool isRedownloaded = false;

            // If this is a video, then only update the captions and keywords.
            switch (runtimeFlags.SourceRedownload)
            {
                case OperationLevel.Normal:
                    if ((sourceMetadata.IsVideo) && !runtimeFlags.IncludeVideos)
                    {
                        Trace.WriteLine("    >? Redownload to Source Suppressed (Photos Only): " + sourceImage.FullFileName);
                    }
                    else
                    {
                        Trace.WriteLine("    > Redownloading (remote more recent): " + sourceImage.FullFileName);
                        var smugImage = await _smCore.AlbumImageService.GetImageDetail(targetImage.AlbumKey, targetImage.ImageKey, targetImage.Serial);
                        isRedownloaded = await _smCore.AlbumImageService.DownloadPrimaryImage(smugImage, sourceImage.FullFileName);
                    }
                    break;
                case OperationLevel.NoneLog:
                    Trace.WriteLine("    >? Redownload to Source Suppressed: " + sourceImage.FullFileName);
                    break;
                default:
                    break;
            }

            return isRedownloaded;
        }

        public virtual  async Task<bool> UpdateMetadata(RuntimeFlagsConfig runtimeFlags, FileMetaContent sourceXmlMetadata, AlbumImageDetail targetImage)
        {
            // Scenario 3: Not forcing binary update, but will always update metadata if different
            bool isMetadataUpdated = false;

            bool isMetadataDifferent =
                _smCore.ContentMetadataService.AreKeywordsDifferent(sourceXmlMetadata, targetImage.Keywords) ||
                ((sourceXmlMetadata.Title ?? "") != System.Web.HttpUtility.HtmlDecode(targetImage.Title ?? "")) ||
                ((sourceXmlMetadata.Caption ?? "") != System.Web.HttpUtility.HtmlDecode(targetImage.Caption ?? ""));

            if (isMetadataDifferent)
            {
                targetImage.Title = sourceXmlMetadata.Title;
                targetImage.Caption = sourceXmlMetadata.Caption;
                targetImage.Keywords = string.Join(";", sourceXmlMetadata.Keywords);

                // If this is a video, then only update the titles and keywords.
                switch (runtimeFlags.TargetUpdate)
                {
                    case OperationLevel.Normal:
                        if (sourceXmlMetadata.IsVideo && (!runtimeFlags.IncludeVideos))
                        {
                            Trace.WriteLine("    >? Suppressing Meta Update (Photos Only): " + targetImage.FileName);
                        }
                        else
                        {
                            Trace.WriteLine("    > Updating Metadata: " + targetImage.FileName);
                            _ = await _smCore.AlbumImageService.UpdateAlbumImage(targetImage);
                            return true;
                        }
                        break;
                    case OperationLevel.NoneLog:
                        Trace.WriteLine("    >? Updating Metadata Suppressed: " + targetImage.FileName);
                        break;
                    default:
                        break;
                }
            }

            return isMetadataUpdated;
        }


        public async static Task<bool> IsRedownloadNeeded(SmugMugCore.Net.Core20.SmugMugCore core, SourceMediaData sourceImage, AlbumImageDetail targetImage)
        {
            var smugImage = await core.AlbumImageService.GetImageDetail(targetImage.AlbumKey, targetImage.ImageKey, targetImage.Serial);
            if (smugImage.LastUpdated == null)
            {
                return true;
            }
            else
            {
                // TODO: need to fix this.
                if (smugImage.LastUpdated != null)
                {
                    TimeSpan tsUpdated = sourceImage.LastWriteTime.Subtract(ConvertFromDateTimeOffset((DateTimeOffset)smugImage.LastUpdated));
                    if ((tsUpdated.TotalMilliseconds < 0) && smugImage.ArchivedMD5 != (await sourceImage.LoadMd5Checksum()))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private static DateTime ConvertFromDateTimeOffset(DateTimeOffset dateTime)
        {
            if (dateTime.Offset.Equals(TimeSpan.Zero))
                return dateTime.UtcDateTime;
            else if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
                return DateTime.SpecifyKind(dateTime.DateTime, DateTimeKind.Local);
            else
                return dateTime.DateTime;
        }

        private async static Task<bool> UploadNewMedia(SmugMugCore.Net.Core20.SmugMugCore core, AlbumDetail targetAlbum, FileMetaContent targetMetadata)
        {
            return await UploadMedia(core, targetAlbum, null, targetMetadata);
        }

        private async static Task<bool> UploadMedia(SmugMugCore.Net.Core20.SmugMugCore core, AlbumDetail targetAlbum, AlbumImageDetail? targetImage, FileMetaContent targetMetadata)
        {
            string targetImageKey = String.Empty;
            int targetImageSerial = 0;
            if (targetImage != null)
            {
                targetImageKey = targetImage.ImageKey;
                targetImageSerial = targetImage.Serial;
            }

            try
            {
                // Attempt #1
                if (targetImage == null)
                    _ = await core.ImageUploaderService.UploadAlbumImage(targetAlbum.AlbumKey, targetMetadata);
                else
                    _ = await core.ImageUploaderService.UploadAlbumImage(targetAlbum.AlbumKey, targetImageKey, targetImageSerial, targetMetadata);

                return true;
            }
            catch (HttpRequestException httpReqEx)
            {
                // Non-SmugMugException - Retry just once (possibly network related)
                Trace.WriteLine($"    > RETRY (Failed: {targetMetadata.FileInfo?.Name}) = {httpReqEx.Message}");

                System.Threading.Thread.Sleep(1000);

                if (targetImage == null)
                    _ = await core.ImageUploaderService.UploadAlbumImage(targetAlbum.AlbumKey, targetMetadata);
                else
                    _ = await core.ImageUploaderService.UploadAlbumImage(targetAlbum.AlbumKey, targetImageKey, targetImageSerial, targetMetadata);
                return true;
            }
            catch (SmugMugException smugEx)
            {
                // Unknown File Type - skip
                if (smugEx.ErrorCode == 64)
                {
                    Trace.WriteLine("    > ERROR 64 (Invalid File Type): " + smugEx.ErrorMessage);
                    throw new ApplicationException("Invalid File Type");
                }
                else
                {
                    // Attempt #2 (then blow up and escalate higher)
                    Trace.WriteLine("    > RETRY ERROR " + smugEx.ErrorCode + " Query=" + smugEx.ErrorMessage);

                    System.Threading.Thread.Sleep(1000);

                    if (targetImage == null)
                        _ = await core.ImageUploaderService.UploadAlbumImage(targetAlbum.AlbumKey, targetMetadata);
                    else
                        _ = await core.ImageUploaderService.UploadAlbumImage(targetAlbum.AlbumKey, targetImageKey, targetImageSerial, targetMetadata);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Attempt #2 (then blow up and escalate higher)
                Trace.WriteLine($"    > RETRY (Failed: {targetMetadata.FileInfo?.Name}) = {ex.Message}");

                System.Threading.Thread.Sleep(1000);

                if (targetImage == null)
                    _ = await core.ImageUploaderService.UploadAlbumImage(targetAlbum.AlbumKey, targetMetadata);
                else
                    _ = await core.ImageUploaderService.UploadAlbumImage(targetAlbum.AlbumKey, targetImageKey, targetImageSerial, targetMetadata);
                return true;
            }
        }


        private async Task<bool> LoadRemoteAlbums()
        {
            // Clear any existing albums first
            _targetAlbums.Clear();

            var albumService = _smCore.AlbumService;
            var albumList = (from x in (await albumService.GetAlbumListNamesOnly(searchText: _folderSyncPathsConfig.FilterFolderName))
                                            where x.Name != null 
                                            && x.Name.ToUpper().Contains(_folderSyncPathsConfig.FilterFolderName.ToUpper())
                                            select x);
                                            
            // Populate the albums
            if (albumList.Any())
            {
                foreach (AlbumDetail album in albumList)
                {
                    if (album.AlbumKey != null)
                        _targetAlbums.TryAdd(album.AlbumKey, album);
                }
            }

            return true;
        }
    }
}
