﻿using Microsoft.WindowsAPICodePack.Dialogs;
using SmugMug.Net.Core;
using SmugMug.Net.Data;
using SmugMug.Net.Data.Domain.Album;
using SmugMug.Net.Service;
using SmugMugCoreSync.Configuration;
using SmugMugCoreSync.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
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
    internal class TargetAlbumRepository
    {
        private readonly FolderSyncPathsConfig _folderSyncPathsConfig;
        private readonly SmugMugCore _smCore;
        private readonly Dictionary<string, AlbumDetail> _targetAlbums = new();

        public TargetAlbumRepository(SmugMugCore core, FolderSyncPathsConfig folderConfig)
        {
            _smCore = core;
            _folderSyncPathsConfig = folderConfig;
        }

        public void PopulateTargetAlbums()
        {
            Trace.Write("Populating Target Albums...");
            LoadRemoteAlbums();
            Trace.WriteLine($"{_targetAlbums.Count} found.");
        }

        internal void VerifyLinkedFolders(RuntimeFlagsConfig runtimeFlags, SourceFolderRepository sourceFolders)
        {
            foreach (var f in sourceFolders.RetrieveLinkedFolders())
            {
                if (!_targetAlbums.ContainsKey(f.AlbumKey))
                {
                    // Folder is marked as linkedto an album which does not  match
                    f.UnlinkFromAlbum();
                    sourceFolders.RemoveLinkedFolder(f);
                }
            }
        }

        internal void ResyncAlbumTitlesFromFolderNames(RuntimeFlagsConfig runtimeFlags, SourceFolderRepository sourceFolders)
        {
            foreach (AlbumDetail album in _targetAlbums.Values)
            {
                string? albumTitle = sourceFolders.RetrieveLinkedFolderByKey(album.AlbumKey)?.FolderName ?? null;
                if (albumTitle != null && albumTitle != album.Title)
                {
                    switch (runtimeFlags.TargetUpdate)
                    {
                        case OperationLevel.Normal:
                            // Update SmugMug
                            album.Title = albumTitle;
                            //_smCore.AlbumService.UpdateAlbum(album);
                            Trace.WriteLine("Album NOT Renamed (api deprecated): " + albumTitle);
                            break;
                        case OperationLevel.NoneLog:
                            Trace.WriteLine("..? Rename Suppressed: " + albumTitle);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        internal void SyncNewFolders(RuntimeFlagsConfig runtimeFlags, SourceFolderRepository sourceFolders)
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
                        var stubNewAlbum = _smCore.AlbumService.CreateAlbum(alb);
                        var createdAlbum = _smCore.AlbumService.GetAlbumDetail(stubNewAlbum.AlbumId, stubNewAlbum.AlbumKey);
                        
                        // Link the folder to the album, and add the new album to the internal objects
                        f.LinkToAlbum(albumId: stubNewAlbum.AlbumId, albumKey: stubNewAlbum.AlbumKey);
                        _targetAlbums.Add(createdAlbum.AlbumKey, createdAlbum);
                        sourceFolders.AddNewLinkedFolder(f);

                        break;
                    case OperationLevel.NoneLog:
                        Trace.WriteLine($"..? Album Create Suppressed: {alb.Title}");
                        break;
                    default:
                        break;
                }
            }
        }

        internal async Task SyncFolderFiles(RuntimeFlagsConfig runtimeFlags, SourceFolderRepository sourceFolders)
        {
            var uploadThrottler = new SemaphoreSlim(runtimeFlags.ImageUploadThrottle);

            foreach (var targetAlbum in _targetAlbums.Values)
            {
                Trace.WriteLine(" ... " + targetAlbum.Title);
                var sourceFolder = sourceFolders.RetrieveLinkedFolderByKey(targetAlbum.AlbumKey);
                if (sourceFolder == null)
                {
                    Trace.WriteLine("... X Source folder is not found / linked to this album.");
                    continue;
                }

                //
                // Load the source files and album images for the current linked album 
                //
                var sourceFiles = sourceFolders.LoadFolderMediaFiles(sourceFolder);

                AlbumDetail albumImages = _smCore.ImageService.GetAlbumImages(albumId: targetAlbum.AlbumId, albumKey: targetAlbum.AlbumKey,
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
                            _smCore.ImageService.Delete(targetImage.ImageId, targetAlbum.AlbumId);
                            break;
                        case OperationLevel.NoneLog:
                            Trace.WriteLine("..? Delete Suppressed: " + targetImage.Title);
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
                            _smCore.ImageService.Delete(toDeleteImage.ImageId, targetAlbum.AlbumId);
                            break;
                        case OperationLevel.NoneLog:
                            Trace.WriteLine("..? Delete of Dupe Suppressed: " + toDeleteImage.Title);
                            break;
                        default:
                            break;
                    }
                    toDeleteImage.IsDeleted = true;
                }

                // Add new files
                var taskList = new List<Task>();
                foreach (var name in newFilenames)
                {
                    var sourceImage = sourceFileNameLookup[name];
                    Trace.WriteLine($"Process {sourceImage.FileName}");
                    taskList.Add(ProcessNewRemoteMedia(runtimeFlags, sourceImage, targetAlbum, uploadThrottler));    
                }

                // Check existing files to see if they need to be updated
                foreach (var name in existingFilenames)
                {
                    var sourceImage = sourceFileNameLookup[name];
                    var targetImage = targetFileNameLookup[name];

                    if (!targetImage.IsDeleted)
                    {
                        taskList.Add(ProcessExistingRemoteMedia(runtimeFlags, sourceImage, targetAlbum, targetImage, uploadThrottler));
                    }
                }
                Task.WaitAll(taskList.ToArray());
            }
        }

        private async Task ProcessExistingRemoteMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, AlbumDetail targetAlbum, ImageDetail targetImage, SemaphoreSlim uploadThrottler)
        {
            var reDownloadImage = false;

            // Load the metadata for the existing source image
            ImageContent sourceMetadata = ContentMetadataLoader.DiscoverMetadata(sourceImage.FullFileName);

            await uploadThrottler.WaitAsync();
            try
            {

                // Scenario 1: Filelength = 0; Redownload
                if (!reDownloadImage && (sourceImage.FileLength == 0))
                {
                    Trace.WriteLine(" ** 0 Byte File Found on source: " + sourceImage.FileName);
                    reDownloadImage = true;
                }

                // Scenario 2: Filelength is different (targetupdate)
                if (!reDownloadImage && sourceImage.IsImageUpdateable() &&
                    (sourceImage.FileLength != targetImage.SizeBytes) || runtimeFlags.ForceRefresh)
                {
                    var imageReuploaded = await RefreshRemoteMedia(runtimeFlags, sourceImage, sourceMetadata, targetAlbum, targetImage);
                    if (imageReuploaded)
                    {
                        reDownloadImage = IsRedownloadNeeded(_smCore, sourceImage, targetImage);
                    }
                }
                else
                {
                    UpdateMetadata(runtimeFlags, sourceMetadata, targetImage);
                    reDownloadImage = false;
                }

                // If the MD5 Image is not matching, but should, we redownload the image with a smugmug in the filename 
                // to provide the option of viewing the different image (and possibly overwrite to have it match)
                if (reDownloadImage)
                {
                    RedownloadMedia(runtimeFlags, sourceImage, targetImage);
                }
            }
            finally
            {
                uploadThrottler.Release();
            }
        } 

        private async Task<bool> ProcessNewRemoteMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, AlbumDetail targetAlbum, SemaphoreSlim uploadThrottler)
        {
            bool continueToAdd = true;

            var sourceMetadata = ContentMetadataLoader.DiscoverMetadata(sourceImage.FullFileName);

            // Smugmug does not support Titles yet.
            sourceMetadata.Caption = sourceMetadata.Title;
            sourceMetadata.Title = null;

            if (sourceMetadata.IsVideo)
            {
                if (sourceMetadata.VideoLength.TotalMinutes > 20)
                {
                    Trace.WriteLine(" ... SKIP (Video too long, > 20 minutes): " + sourceImage.FileName);
                    continueToAdd = false;
                }
                
                if (sourceMetadata.FileInfo?.Length > 2000000000)
                {
                    Trace.WriteLine(" ... SKIP (Video size too large, > 2.0 gigs): " + sourceImage.FileName);
                    continueToAdd = false;
                }
                
                if (!runtimeFlags.IncludeVideos)
                {
                    Trace.WriteLine(" ... SKIP (Photos Only Enabled): " + sourceImage.FileName);
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
                            await UploadNewMedia(_smCore, targetAlbum, sourceMetadata);
                            Trace.WriteLine(" ... Uploaded " + sourceImage.FileName);
                        }
                        finally
                        {
                            uploadThrottler.Release();
                        }

                        return true;
                    case OperationLevel.NoneLog:
                        Trace.WriteLine("..? Upload Suppressed: " + sourceImage.FileName);
                        break;
                    default:
                        break;
                }
            }

            return false;
        }

        private async Task<bool> RefreshRemoteMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, ImageContent sourceMetadata, AlbumDetail targetAlbum, ImageDetail targetImage)
        {
            sourceMetadata.MD5Checksum = sourceImage.LoadMd5Checksum();

            if ((sourceMetadata.MD5Checksum != targetImage.MD5Sum))
            {
                switch (runtimeFlags.TargetUpdate)
                {
                    case OperationLevel.Normal:
                        if ((sourceMetadata.IsVideo) && runtimeFlags.IncludeVideos)
                        {
                            Trace.WriteLine("..? Update Suppressed (Photos Only): " + sourceImage.FileName);
                        }
                        else
                        {
                            await UploadMedia(_smCore, targetAlbum, targetImage, sourceMetadata);
                            return true;
                        }
                        break;
                    case OperationLevel.NoneLog:
                        Trace.WriteLine("..? Update Suppressed: " + sourceImage.FileName);
                        break;
                    default:
                        break;
                }
            }

            return false;
        }

        private void RedownloadMedia(RuntimeFlagsConfig runtimeFlags, SourceMediaData sourceImage, ImageDetail targetImage)
        {
            Trace.WriteLine(" ** Redownload Image Requested: " + sourceImage.FullFileName);
            var smugImage = _smCore.ImageService.GetImageInfo(targetImage.ImageId, targetImage.ImageKey);

            // If this is a video, then only update the captions and keywords.
            switch (runtimeFlags.SourceVideoRedownload)
            {
                case OperationLevel.Normal:
                    Trace.WriteLine("... Redownloading: " + sourceImage.FileName);
                    _smCore.ImageService.DownloadImage(smugImage, sourceImage.FileName);
                    break;
                case OperationLevel.NoneLog:
                    Trace.WriteLine("..? Update to Source Suppressed: " + sourceImage.FileName);
                    break;
                default:
                    break;
            }
        }

        private bool UpdateMetadata(RuntimeFlagsConfig runtimeFlags, ImageContent sourceXmlMetadata, ImageDetail targetImage)
        {
            // Scenario 3: Not forcing binary update, but will always update metadata if different
            bool isMetadataDifferent =
                ContentMetadataLoader.AreKeywordsDifferent(sourceXmlMetadata, targetImage.Keywords) ||
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
                            Trace.WriteLine("... Suppressing Upload (Photos Only): " + targetImage.Filename);
                        }
                        else
                        {
                            Trace.WriteLine("... Updating Metadata: " + targetImage.Filename);
                            _smCore.ImageService.UpdateImage(targetImage);
                            return true;
                        }
                        break;
                    case OperationLevel.NoneLog:
                        Trace.WriteLine("..? Update Suppressed: " + targetImage.Filename);
                        break;
                    default:
                        break;
                }
            }

            return false;
        }

        private static bool IsRedownloadNeeded(SmugMugCore core, SourceMediaData sourceImage, ImageDetail targetImage)
        {
            // Now get the MD5 and see if it matches...
            var smugImage = core.ImageService.GetImageInfo(targetImage.ImageId, targetImage.ImageKey);
            if (smugImage.LastUpdatedDate == null)
            {
                return true;
            }
            else
            {
                var tsUpdated = sourceImage.LastWriteTime - DateTime.Parse(smugImage.LastUpdatedDate);
                if (smugImage.MD5Sum != sourceImage.LoadMd5Checksum() && (tsUpdated.TotalMilliseconds < 0))
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
                return await core.ImageUploaderService.UploadUpdatedImageAsync(targetAlbum.AlbumId, targetImageId, targetMetadata);
            }
            catch (AggregateException agg)
            {
                SmugMugException? smugex = agg.InnerExceptions[0] as SmugMugException;
                WebException? webEx = agg.InnerExceptions[0] as WebException;
                
                if (smugex != null)
                {
                    // Unknown File Type - skip
                    if (smugex.ErrorResponse.Code == 64)
                    {
                        Trace.WriteLine("... SKIP - ERROR 64 (Invalid File Type): " + smugex.QueryString);
                        throw new ApplicationException("Invalid File Type");
                    }
                    else
                    {
                        // Attempt #2 (then blow up and escalate higher)
                        Trace.WriteLine("... RETRY - ERROR " + smugex.ErrorResponse.Code + " Query=" + smugex.QueryString);
                        System.Threading.Thread.Sleep(1000);
                        return await core.ImageUploaderService.UploadUpdatedImageAsync(targetAlbum.AlbumId, targetImageId, targetMetadata);
                    }
                }
                else if (webEx != null)
                {
                    // Non-Smugmug Exception - Retry just once (possibly network related)
                    Trace.WriteLine("... RETRY, WebException = " + webEx.Message);
                    System.Threading.Thread.Sleep(1000);
                    return await core.ImageUploaderService.UploadUpdatedImageAsync(targetAlbum.AlbumId, targetImageId, targetMetadata);
                }
                else
                {
                    throw;                    
                }
            }
        }


        internal void SyncExistingFolders(RuntimeFlagsConfig runtimeFlags, SourceFolderRepository sourceFolders)
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
                            if (_smCore.AlbumService.DeleteAlbum(_targetAlbums[albumKey].AlbumId))
                            {
                                _targetAlbums.Remove(albumKey);
                            }
                            break;
                        case OperationLevel.NoneLog:
                            Trace.WriteLine($"..? Album Delete Suppressed: {albumKey} / {_targetAlbums[albumKey].Title}");
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void LoadRemoteAlbums()
        {
            // Clear any existing albums first
            _targetAlbums.Clear();

            var albumService = _smCore.AlbumService;
            var albumList = (from x in albumService.GetAlbumList(fieldList: new string[] { "Title" }, returnEmpty: true)
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
        }
    }
}