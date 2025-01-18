using Microsoft.WindowsAPICodePack.Dialogs;
using SmugMug.Net.Core;
using SmugMug.Net.Data;
using SmugMug.Net.Data.Domain.Album;
using SmugMug.Net.Service;
using SmugMugCoreSync.Configuration;
using SmugMugCoreSync.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace SmugMugCoreSync.Repositories
{
    public class TargetAlbumRepository
    {
        private readonly FolderSyncPathsConfig _folderSyncPathsConfig;
        private readonly SmugMugCore _smCore;
        private readonly Dictionary<string, AlbumDetail> _targetAlbums = new();

        public TargetAlbumRepository(SmugMugCore core, FolderSyncPathsConfig folderConfig)
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
                var alb = new AlbumDetail()
                {
                    SortDirectionDescending = true,
                    SortMethod = SortMethod.DateTimeOriginal,
                    ViewingLargeImagesEnabled = true,
                    ViewingLargeX2ImagesEnabled = true,
                    ViewingLargeX3ImagesEnabled = true,
                    ViewingLargeXImagesEnabled = true,
                    ViewingOriginalImagesEnabled = true,
                    ExifAllowed = true,
                    CommentsAllowed = true,
                    CanRank = true,
                    ShareEnabled = true,
                    PublicDisplay = true,
                    GeographyMappingEnabled = false,
                    Title = f.FolderName
                };

                switch (runtimeFlags.TargetCreate)
                {
                    case OperationLevel.Normal:
                        Trace.WriteLine($"    > Creating Album: {alb.Title}");
                        var stubNewAlbum = await _smCore.AlbumService.CreateAlbum(alb);
                        var createdAlbum = await _smCore.AlbumService.GetAlbumDetail(stubNewAlbum.AlbumId, stubNewAlbum.AlbumKey);
                        
                        // Link the folder to the album, and add the new album to the internal objects
                        f.LinkToAlbum(albumId: stubNewAlbum.AlbumId, albumKey: stubNewAlbum.AlbumKey);
                        _targetAlbums.Add(createdAlbum.AlbumKey, createdAlbum);
                        sourceFolders.AddNewLinkedFolder(f);

                        break;
                    case OperationLevel.NoneLog:
                        Trace.WriteLine($"    >? Album Create Suppressed: {alb.Title}");
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
                            Trace.WriteLine($"    > Deleting Album: {albumKey} / {_targetAlbums[albumKey].Title}");
                            if (await _smCore.AlbumService.DeleteAlbum(_targetAlbums[albumKey].AlbumId))
                            {
                                _targetAlbums.Remove(albumKey);
                            }
                            break;
                        case OperationLevel.NoneLog:
                            Trace.WriteLine($"    >? Album Delete Suppressed: {albumKey} / {_targetAlbums[albumKey].Title}");
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

            var albumSortedList = _targetAlbums.Values.OrderBy(x => x.Title);
            foreach (var targetAlbum in albumSortedList)
            {
                runtimeStats.ProcessedFolders++;
                
                Trace.WriteLine($"  Album: {targetAlbum.Title}");
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
                runtimeFileStats.FolderName = targetAlbum.Title ?? String.Empty;
                
                var sourceFiles = sourceFolders.LoadFolderMediaFiles(sourceFolder);

                AlbumDetail albumImages = await _smCore.ImageService.GetAlbumImages(albumId: targetAlbum.AlbumId, albumKey: targetAlbum.AlbumKey,
                    fieldList: new string[] { "Filename", "Name", "Title", "Caption", "SizeBytes", "MD5Sum", "Keywords" });
                ImageDetail[] targetFiles = albumImages.Images ?? Array.Empty<ImageDetail>();

                //
                // Build a lookup for source files, by a cleaned up filename for indexing
                //
                Dictionary<string, SourceMediaData> sourceFileNameLookup = new();
                Dictionary<string, ImageDetail> targetFileNameLookup = new();

                string[] deleteFilenames;
                string[] newFilenames;
                string[] existingFilenames;
                ImageDetail[] dupeTargetImages;

                foreach (var file in sourceFiles)
                {
                    sourceFileNameLookup.Add(file.FileNameBase, file);
                }
                List<ImageDetail> dupeFileNameList = new();
                foreach (var file in targetFiles)
                {
                    if (targetFileNameLookup.ContainsKey(file.FileNameBase))
                        dupeFileNameList.Add(file);
                    else
                        targetFileNameLookup.Add(file.FileNameBase, file);
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
                            Trace.WriteLine($"    > Deleting Remote Media: {targetImage.ImageId} / {targetName}");
                            _ = await _smCore.ImageService.Delete(targetImage.ImageId, targetAlbum.AlbumId);
                            runtimeFileStats.DeletedFiles++;
                            break;
                        case OperationLevel.NoneLog:
                            Trace.WriteLine($"    >? Delete Suppressed: {targetImage.ImageId} / {targetName}");
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
                            _ = await _smCore.ImageService.Delete(toDeleteImage.ImageId, targetAlbum.AlbumId);
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

        public async virtual Task<bool> ProcessExistingRemoteMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, AlbumDetail targetAlbum, ImageDetail targetImage, SemaphoreSlim uploadThrottler)
        {
            var reDownloadImage = false;

            // Load the metadata for the existing source image
            ContentMetadataService metaSvc = _smCore.ContentMetadataService;
            metaSvc.FileSystem = sourceImage.FileSystem;
            ImageContent sourceMetadata = await metaSvc.DiscoverMetadata(sourceImage.FullFileName);

            // SmugMug does not support Titles yet.
            sourceMetadata.Caption = sourceMetadata.Title;
            sourceMetadata.Title = null;

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
                    if ((sourceImage.FileLength != targetImage.SizeBytes) || runtimeFlags.ForceRefresh)
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
            ImageContent sourceMetadata = await metaSvc.DiscoverMetadata(sourceImage.FullFileName);

            // SmugMug does not support Titles yet.
            sourceMetadata.Caption = sourceMetadata.Title;
            sourceMetadata.Title = null;

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
                            await UploadNewMedia(_smCore, targetAlbum, sourceMetadata);
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

        public virtual async Task<bool> RefreshRemoteMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, ImageContent sourceMetadata, AlbumDetail targetAlbum, ImageDetail targetImage)
        {
            if (sourceMetadata.IsVideo)
            {
                // Per validation with SmugMug, and integration tests, we cannot update a video
                // already uploaded. Would have to create+publish a whole new video (which I will not do automatically)
                Trace.WriteLine("    > SKIP Update not supported for Videos: " + sourceImage.FileName);
                return false;
            }

            sourceMetadata.MD5Checksum = await sourceImage.LoadMd5Checksum();
            if ((sourceMetadata.MD5Checksum != targetImage.MD5Sum))
            {
                switch (runtimeFlags.TargetUpdate)
                {
                    case OperationLevel.Normal:
                        Trace.WriteLine("    > Updating Remote Media: " + sourceImage.FileName);
                        await UploadMedia(_smCore, targetAlbum, targetImage, sourceMetadata);
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

        public virtual  async Task<bool> RedownloadMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, ImageContent sourceMetadata, ImageDetail targetImage)
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
                        var smugImage = await _smCore.ImageService.GetImageInfo(targetImage.ImageId, targetImage.ImageKey);
                        isRedownloaded = await _smCore.ImageService.DownloadImage(smugImage, sourceImage.FullFileName);
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

        public virtual  async Task<bool> UpdateMetadata(RuntimeFlagsConfig runtimeFlags, ImageContent sourceXmlMetadata, ImageDetail targetImage)
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
                            Trace.WriteLine("    >? Suppressing Meta Update (Photos Only): " + targetImage.Filename);
                        }
                        else
                        {
                            Trace.WriteLine("    > Updating Metadata: " + targetImage.Filename);
                            isMetadataUpdated = await _smCore.ImageService.UpdateImage(targetImage);
                            return true;
                        }
                        break;
                    case OperationLevel.NoneLog:
                        Trace.WriteLine("    >? Updating Metadata Suppressed: " + targetImage.Filename);
                        break;
                    default:
                        break;
                }
            }

            return isMetadataUpdated;
        }

        public async static Task<bool> IsRedownloadNeeded(SmugMugCore core, SourceMediaData sourceImage, ImageDetail targetImage)
        {
            var smugImage = await core.ImageService.GetImageInfo(targetImage.ImageId, targetImage.ImageKey);
            if (smugImage.LastUpdatedDate == null)
            {
                return true;
            }
            else
            {
                var tsUpdated = sourceImage.LastWriteTime - DateTime.Parse(smugImage.LastUpdatedDate);
                if ((tsUpdated.TotalMilliseconds < 0) && smugImage.MD5Sum != (await sourceImage.LoadMd5Checksum()))
                {
                    return true;
                }
            }

            return false;
        }

        private async static Task<ImageUpload> UploadNewMedia(SmugMugCore core, AlbumDetail targetAlbum, ImageContent targetMetadata)
        {
            return await UploadMedia(core, targetAlbum, null, targetMetadata);
        }

        private async static Task<ImageUpload> UploadMedia(SmugMugCore core, AlbumDetail targetAlbum, ImageDetail? targetImage, ImageContent targetMetadata)
        {
            long targetImageId = 0;
            if (targetImage != null)
            {
                targetImageId = targetImage.ImageId;
            }
            
            try
            {
                // Attempt #1
                return await core.ImageUploaderService.UploadUpdatedImage(targetAlbum.AlbumId, targetImageId, targetMetadata);
            }
            catch (HttpRequestException httpReqEx)
            {
                // Non-SmugMugException - Retry just once (possibly network related)
                Trace.WriteLine($"    > RETRY (Failed: {targetMetadata?.FileInfo?.Name}) = {httpReqEx.Message}");
                System.Threading.Thread.Sleep(1000);
                return await core.ImageUploaderService.UploadUpdatedImage(targetAlbum.AlbumId, targetImageId, targetMetadata);
            }
            catch (SmugMugException smugEx)
            {
                // Unknown File Type - skip
                if (smugEx.ErrorResponse.Code == 64)
                {
                    Trace.WriteLine("    > ERROR 64 (Invalid File Type): " + smugEx.QueryString);
                    throw new ApplicationException("Invalid File Type");
                }
                else
                {
                    // Attempt #2 (then blow up and escalate higher)
                    Trace.WriteLine("    > RETRY ERROR " + smugEx.ErrorResponse.Code + " Query=" + smugEx.QueryString);
                    System.Threading.Thread.Sleep(1000);
                    return await core.ImageUploaderService.UploadUpdatedImage(targetAlbum.AlbumId, targetImageId, targetMetadata);
                }
            }
            catch (Exception ex)
            {
                // Attempt #2 (then blow up and escalate higher)
                Trace.WriteLine($"    > RETRY (Failed: {targetMetadata?.FileInfo?.Name}) = {ex.Message}");
                System.Threading.Thread.Sleep(1000);
                return await core.ImageUploaderService.UploadUpdatedImage(targetAlbum.AlbumId, targetImageId, targetMetadata);
            }
        }


        private async Task<bool> LoadRemoteAlbums()
        {
            // Clear any existing albums first
            _targetAlbums.Clear();

            var albumService = _smCore.AlbumService;
            var albumList = (from x in (await albumService.GetAlbumList(fieldList: new string[] { "Title" }))
                                where x.Title != null && x.Title.Contains('-')
                                && x.Title.ToUpper().Contains(_folderSyncPathsConfig.FilterFolderName.ToUpper())
                                select x);

            // Populate the albums
            if (albumList.Any())
            {
                foreach (AlbumDetail album in albumList)
                {
                    if (album.AlbumKey != null)
                        _targetAlbums.Add(album.AlbumKey, album);
                }
            }

            return true;
        }
    }
}
