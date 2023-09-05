using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;
using SmugMug.Net.Data.Domain.Image;

namespace SmugMug.Net.Service
{
    public class ImageService
    {
        private readonly Core.SmugMugCore _core;

        public ImageService(Core.SmugMugCore core)
        {
            _core = core;
        }


        /// <summary>
        /// Applies a watermark to an image
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="watermarkId">The id for a specific watermark</param>
        /// <returns></returns>
        public async virtual Task<bool> ApplyWatermark(long imageId, int watermarkId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("WatermarkID", watermarkId);

            _ = await _core.QueryWebsite<Data.SmugmugError>("smugmug.images.applyWatermark", queryParams, false);

            // Return Results
            return true;
        }


        /// <summary>
        /// Change a sorting position of an image in an album
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="position">The position of the image (or video) within the album</param>
        /// <returns></returns>
        public async virtual Task<bool> ChangePosition(long imageId, int position)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("Position", position);

            _ = await _core.QueryWebsite<Data.SmugmugError>("smugmug.images.changePosition", queryParams, false);

            // Return Results
            return true;
        }

        /// <summary>
        /// Download an image from Smugmug Locally
        /// </summary>
        /// <param name="image"></param>
        /// <param name="localPath"></param>
        public async virtual Task<bool> DownloadImage(Data.ImageDetail image, string localPath)
        {
            if (image.UrlViewOriginalURL == null)
                return false;

            return await _core.DownloadContentAsync(image.UrlViewOriginalURL, localPath);
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
        /// Move an image to a different album
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="imageKey">The key for a specific image</param>
        /// <param name="albumId">The id for a specific album</param>
        /// <returns>True if successful</returns>
        public async virtual Task<bool> MoveToAlbum(long imageId, string imageKey, int albumId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("ImageKey", imageKey);
            queryParams.Add("AlbumID", albumId);

            _ = await _core.QueryWebsite<Data.ImageDetail>("smugmug.images.collect", queryParams, false);

            // Return Results
            return true;
        }


        /// <summary>
        /// Add a comment to an image
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="imageKey">The key for a specific image</param>
        /// <param name="comment">The text for the comment</param>
        /// <param name="rating">The rating for the comment. 
        /// Values: 0 - No Rating (default), 1 - 1 Star Rating, 2 - 2 Star Rating, 3 - 3 Star Rating, 4 - 4 Star Rating, 5 - 5 Star Rating</param>
        /// <returns></returns>
        public async virtual Task<Data.Comment> AddComment(string[] fieldList, long imageId, string imageKey, string comment, int rating = 0)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("ImageKey", imageKey);
            queryParams.Add("Text", comment);
            queryParams.Add("Rating", rating, 0);
            queryParams.Add("Extras", Core.SmugMugCore.ConvertFieldListToXmlFields<Data.Comment>(fieldList) ?? "", "");

            var response = await _core.QueryWebsite<Data.Comment>("smugmug.images.comments.add", queryParams, false);
            var retVal = response[0];
            retVal.Text = comment;
            retVal.Rating = rating;
            return retVal;
        }

        /// <summary>
        /// Retrieve a list of comments for an image
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="imageKey">The key for a specific image</param>
        /// <param name="albumPassword">The password for the album</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
        public async virtual Task<Data.Comment[]> GetCommentList(long imageId, string imageKey, string albumPassword = "", string sitePassword = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("ImageKey", imageKey);
            queryParams.Add("Password", albumPassword, "");
            queryParams.Add("SitePassword", sitePassword, "");

            var response = await _core.QueryWebsite<Data.ImageDetail>("smugmug.images.comments.get", queryParams, false);
            if (response != null && response.Length > 0)
            {
                var responseDetail = response[0];
                if (responseDetail != null && responseDetail.Comments != null)
                    return responseDetail.Comments;
            }

            // Return Results
            return Array.Empty<Data.Comment>();
        }

        /// <summary>
        /// Crap an image in an album
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="height">The height of the crop</param>
        /// <param name="width">The width of the crop</param>
        /// <param name="x">The x coordinate of the starting point</param>
        /// <param name="y">The y coordinate of the starting point</param>
        /// <returns></returns>
        public async virtual Task<bool> Crop(long imageId, int height, int width, int x = 0, int y = 0)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("Height", height);
            queryParams.Add("Width", width);
            queryParams.Add("X", x, 0);
            queryParams.Add("Y", y, 0);

            _ = await _core.QueryWebsite<Data.SmugmugError>("smugmug.images.crop", queryParams, false);

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

            _ = await _core.QueryWebsite<Data.PrintmarkInfo>("smugmug.images.delete", queryParams, false);

            // Return Results
            return true;
        }

        /// <summary>
        /// Retrieve a list of images for an album.
        /// </summary>
        /// <param name="albumId">The id for a specific album</param>
        /// <param name="albumKey">The key for a specific album</param>
        /// <param name="fieldList">List of fields from Images within the Album to add to the resultset</param>
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
        /// <param name="fieldList">List of fields from Images within the Album to add to the resultset</param>
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
        /// Retrieve the EXIF data for an image
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="imageKey">The key for a specific image</param>
        /// <param name="albumPassword">The password for the album</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <returns></returns>
        public async virtual Task<Data.ImageExif> GetImageExif(long imageId, string imageKey, string albumPassword = "", string sitePassword = "")
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("ImageKey", imageKey);
            queryParams.Add("Password", albumPassword, "");
            queryParams.Add("SitePassword", sitePassword, "");

            var response = await _core.QueryWebsite<Data.ImageExif>("smugmug.images.getEXIF", queryParams, false);
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
        /// Retrieve the statistics for an Image
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="month">The month to retrieve statistics for</param>
        /// <param name="year">The year to retrieve statistics for</param>
        /// <returns></returns>
        [ObsoleteAttribute("smugmug.images.getStats.get no longer is working with v1.3.0 Smugmug API.")]
        public async virtual Task<Data.ImageStats> GetImageStats(long imageId, int month, int year)
        {
            // Append the parameters from teh request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("Month", month);
            queryParams.Add("Year", year);

            var queryResponse = await _core.QueryWebsite<Data.ImageStats>("smugmug.images.getStats", queryParams, false);

            // Return Results
            return queryResponse[0];
        }

 
        /// <summary>
        /// Remove a watermark from an image
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <returns></returns>
        public async virtual Task<bool> RemoveWatermark(long imageId)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);

            _ = await _core.QueryWebsite<Data.SmugmugError>("smugmug.images.removeWatermark", queryParams, false);

            // Return Results
            return true;
        }
        /// <summary>
        /// Rotates an image
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="degrees">The degrees of rotation: 90 - Left, 180 - Down, 270 - Right</param>
        /// <param name="flip">Mirror the image in the horizontal direction</param>
        /// <returns></returns>
        public async virtual Task<bool> Rotate(long imageId, Degrees degrees, bool flip)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("Degrees", (int) degrees);
            queryParams.Add("Flip", flip);

            _ = await _core.QueryWebsite<Data.SmugmugError>("smugmug.images.rotate", queryParams, false);

            // Return Results
            return true;
        }


        /// <summary>
        /// Crop the thumbnail of an image
        /// </summary>
        /// <param name="imageId">The id for a specific image</param>
        /// <param name="height">The height of the crop</param>
        /// <param name="width">The width of the crop</param>
        /// <param name="x">The x coordinate of the starting point</param>
        /// <param name="y">The y coordinate of the starting point</param>
        /// <returns></returns>
        public async virtual Task<bool> ZoomThumbnail(long imageId, int height, int width, int x = 0, int y = 0)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("ImageID", imageId);
            queryParams.Add("Height", height);
            queryParams.Add("Width", width);
            queryParams.Add("X", x, 0);
            queryParams.Add("Y", y, 0);

            _ = await _core.QueryWebsite<Data.SmugmugError>("smugmug.images.zoomThumbnail", queryParams, false);

            // Return Results
            return true;
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

            queryParams.Add("Altitude", image.Altitude);
            if (image.Title != null)
                queryParams.Add("Title", image.Title);
            if (image.Caption != null)
                queryParams.Add("Caption", image.Caption);
            if (image.Filename != null)
                queryParams.Add("FileName", image.Filename);
            queryParams.Add("Hidden", image.Hidden);
            if (image.Keywords != null)
                queryParams.Add("Keywords", image.Keywords);
            queryParams.Add("Latitude", image.Latitude);
            queryParams.Add("Longitude", image.Longitude);
        }

    }
}
