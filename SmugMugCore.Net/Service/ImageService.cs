using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMugCore.Net;
using SmugMugCore.Net.Data.Domain.Image;

namespace SmugMugCore.Net.Service
{
    public class ImageService
    {
        private readonly Core.SmugMugCore _core;

        public ImageService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Download an image from Smugmug Locally
        /// </summary>
        /// <param name="image"></param>
        /// <param name="localPath"></param>
        public async virtual Task<bool> DownloadImage(Data.ImageDetail image, string localPath)
        {
            string targetImage;
            if (image.UrlVideoOriginalURL.Length > 0)
                targetImage = image.UrlVideoOriginalURL;
            else if (image.UrlViewOriginalURL.Length > 0)
                targetImage = image.UrlViewOriginalURL;
            else
                return false;

            return await _core.DownloadContentAsync(targetImage, localPath);
        }

        /// <summary>
        /// Change the settings of an image
        /// </summary>
        /// <param name="image">Image to update</param>
        /// <returns></returns>
        public async virtual Task<bool> UpdateImage(Data.ImageDetail image)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", image.ImageId);
            ImageService.AddImageParameters(queryParams, image);

            _ = await _core.QueryWebsite<Data.ImageDetail>("smugmug.images.changeSettings", queryParams, false);

            // Return Results
            return true;
        }

        /// <summary>
        /// Delete image
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="albumId">The id for the album that has the image</param>
        /// <returns></returns>
        public async virtual Task<bool> Delete(long imageId, int albumId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("AlbumID", albumId);

            _ = await _core.QueryWebsite<Data.ImageDetail>("smugmug.images.delete", queryParams, false);

            // Return Results
            return true;
        }

        /// <summary>
        /// Retrieve a list of images for an album.
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="albumKey">The key for a specific album</param>
        /// <param name="fieldList">List of fields from Images within the Album to add to the results</param>
        /// <returns></returns>
        public async virtual Task<Data.AlbumDetail> GetAlbumImages(string[] fieldList, int albumId, string albumKey)
        {
            return await GetAlbumImagesExt(fieldList:fieldList, albumId:albumId, albumKey:albumKey);
        }

        /// <summary>
        /// Retrieve a list of images for an album.
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="albumKey">The key for a specific album</param>
        /// <param name="albumPassword">The password for the album</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <param name="fieldList">List of fields from Images within the Album to add to the results</param>
        /// <returns></returns>
        public async virtual Task<Data.AlbumDetail> GetAlbumImagesExt(string[] fieldList, int albumId, string albumKey, string albumPassword = "", string sitePassword = "", bool loadAllImageInfo = false)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("AlbumID", albumId);
            queryParams.Add("AlbumKey", albumKey);
            queryParams.Add("Password", albumPassword, "");
            queryParams.Add("SitePassword", sitePassword, "");
            queryParams.Add("Extras", Core.SmugMugCore.ConvertFieldListToXmlFields<Data.ImageDetail>(fieldList) ?? "", "");
            queryParams.Add("Heavy", loadAllImageInfo);

            var response = await _core.QueryWebsite<Data.AlbumDetail>("smugmug.images.get", queryParams, false);
            return response[0];
        }

        /// <summary>
        /// Retrieve the information for an image.
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="imageKey">The key for a specific image</param>
        /// <returns></returns>
        public async virtual Task<Data.ImageDetail> GetImageInfo(long imageId, string imageKey)
        {
            return await GetImageInfoExt(imageId, imageKey);
        }

        /// <summary>
        /// Retrieve the information for an image.
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="imageKey">The key for a specific image</param>
        /// <param name="albumPassword">The password for the album</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
        public async virtual Task<Data.ImageDetail> GetImageInfoExt(long imageId, string imageKey, string customSize = "", string albumPassword = "", string sitePassword = "", bool includeOnlyUrls = false)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("ImageKey", imageKey);
            queryParams.Add("CustomSize", customSize, "");
            queryParams.Add("Password", albumPassword, "");
            queryParams.Add("SitePassword", sitePassword, "");

            if (includeOnlyUrls)
            {
                var response = await _core.QueryWebsite<Data.ImageDetail>("smugmug.images.getURLs", queryParams, false);
                return response[0];
            }
            else
            {
                var response = await _core.QueryWebsite<Data.ImageDetail>("smugmug.images.getInfo", queryParams, false);
                return response[0];
            }
        }

        /// <summary>
        /// Add image parameters to the query object for create/modify methods
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="image"></param>
        private static void AddImageParameters(Core.QueryParameterList queryParams, Data.ImageDetail image)
        {
            if (image.Album != null)
                queryParams.Add("AlbumID", image.Album.AlbumId);

            if (image.Title != null)
                queryParams.Add("Title", image.Title);
            if (image.Caption != null)
                queryParams.Add("Caption", image.Caption);
            if (image.Filename != null)
                queryParams.Add("FileName", image.Filename);
            queryParams.Add("Hidden", image.Hidden);
            if (image.Keywords != null)
                queryParams.Add("Keywords", image.Keywords);
        }

    }
}
