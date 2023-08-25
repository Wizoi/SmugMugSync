using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Security.Cryptography;
using System.Net.Http.Headers;

namespace SmugMug.Net.Service
{
    public class UploadEventArgs : EventArgs
    {
        public float PercentComplete { get; internal set; }
        /// <summary>
        /// The filename of the image (or video).
        /// This header overrides whatever is set as the filename in the PUT endpoint.
        /// </summary>
        public string? FileName { get; set; }
    }


    public class ImageUploaderService
    {
        /// <summary>
        /// The API version [required]
        /// </summary>
        private readonly string Version = "1.3.0";
        private readonly Core.SmugMugCore _core;

        public ImageUploaderService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Uploads new image content (no  existing imageid)
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="filename"></param>
        public Data.ImageUpload UploadNewImage(int albumId, Data.ImageContent imageMetadata)
        {
            try
            {
                var imageTask = UploadNewImageAsync(albumId, imageMetadata);
                imageTask.Wait();
                return imageTask.Result;
            }
            catch (AggregateException agg)
            {
                // There will only be one possibe exception in here, so throw the first one. 
                throw agg.InnerExceptions[0];
            }
        }

        /// <summary>
        /// Uploads new image content (no  existing imageid)
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="filename"></param>
        public Data.ImageUpload UploadUpdatedImage(int albumId, long imageId, Data.ImageContent imageMetadata)
        {
            try
            {
                var imageTask = UploadUpdatedImageAsync(albumId: albumId, imageId: imageId, imageMetadata:  imageMetadata);
                imageTask.Wait();
                return imageTask.Result;
            }
            catch (AggregateException agg)
            {
                // There will only be one possibe exception in here, so throw the first one. 
                throw agg.InnerExceptions[0];
            }
        }


        /// <summary>
        /// Uploads new image content (no  existing imageid)
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="filename"></param>
        public async Task<Data.ImageUpload> UploadNewImageAsync(int albumId, Data.ImageContent imageMetadata)
        {
            return await UploadUpdatedImageAsync(albumId, 0, imageMetadata);
        }

        /// <summary>
        /// Uploads the image
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="imageId"></param>
        /// <param name="imageMetadata"></param>
        public async Task<Data.ImageUpload> UploadUpdatedImageAsync(int albumId, long imageId, Data.ImageContent imageMetadata)
        {
            if (imageMetadata.FileInfo == null)
                throw new ApplicationException("Error - Image with no filename detected.");

            if (string.IsNullOrEmpty(imageMetadata.MD5Checksum))
                imageMetadata.MD5Checksum = GetMd5Checksum(imageMetadata.FileInfo);

            var queryData = new Core.QueryParameterList();
            queryData.Add("X-Smug-Version", Version);
            queryData.Add("X-Smug-ResponseType", "REST");
            queryData.Add("X-Smug-AlbumID", albumId);
            queryData.Add("X-Smug-FileName", imageMetadata.FileInfo.Name);
            queryData.Add("Content-MD5", imageMetadata.MD5Checksum);
            /*
             * if (imageContent.Title != null)
                            queryData.Add("X-Smug-Title", imageContent.Title);
            */
            if (imageMetadata.Caption != null)
                queryData.Add("X-Smug-Caption", imageMetadata.Caption);
            if (imageMetadata.IsHidden == true)
                queryData.Add("X-Smug-Hidden", imageMetadata.IsHidden);
            if (imageId > 0)
                queryData.Add("X-Smug-ImageID", imageId);
            if (imageMetadata.Keywords != null)
                queryData.Add("X-Smug-Keywords", string.Join(";", imageMetadata.Keywords));
            if (imageMetadata.GeoAltitude != 0)
                queryData.Add("X-Smug-Altitude", imageMetadata.GeoAltitude.ToString());
            if (imageMetadata.GeoLatitude != 0)
                queryData.Add("X-Smug-Latitude", imageMetadata.GeoLatitude.ToString());
            if (imageMetadata.GeoLongitutde != 0)
                queryData.Add("X-Smug-Longitude", imageMetadata.GeoLongitutde.ToString());

            var fileInfo = imageMetadata.FileInfo;
            string uploadUrl = @"https://upload.smugmug.com/photos/xmlrawadd.mg";

            HttpClient? client = null;
            HttpRequestMessage? request = null;
            HttpResponseMessage? response = null;

            try
            {
                client = _core.BuildHttpClient(new System.Uri(uploadUrl), HttpMethod.Put, null);
                request = _core.BuildHttpRequestMessage(new System.Uri(uploadUrl), HttpMethod.Put, null);

                //- request time out (compute this for 10 kb/sec speed)
                //- the chunk size to use when uploading (how much data to report after)
                int timeOut = ((int)fileInfo.Length / 1024);
                client.Timeout = new TimeSpan(0, 0, timeOut);

                request.Headers.ExpectContinue = false;
                request.Headers.TransferEncodingChunked = true;
                request.Content = new StreamContent(content:fileInfo.OpenRead(), bufferSize:1024 * 6);
                request.Content.Headers.Add("Content-Length", fileInfo.Length.ToString());
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "none");
                queryData.AppendToWebHeaders(request);

                response = await client.SendAsync(request);

                // Done transferring bits, get the response
                if (response.IsSuccessStatusCode)
                {
                    var stream = new System.IO.StreamReader(await response.Content.ReadAsStreamAsync());
                    var imageData = Core.SmugMugCore.DeserializeResponse<Data.ImageUpload>(stream, false, "smugmug.images.upload", queryData, "");
                    return imageData[0];
                }
            }
            finally
            {
                client?.Dispose();
                request?.Dispose();
                response?.Dispose();
            }


            throw new ApplicationException("Image upload to smugmug failed for: " + fileInfo.Name);
        }

        /// <summary>
        /// Add a MD5 Checksum tot he Image Content
        /// </summary>
        /// <param name="imageContent"></param>
        public static string GetMd5Checksum(IFileInfo fileInfo)
        {
            var taskMd5Checksum = GetMd5ChecksumAsync(fileInfo);
            taskMd5Checksum.Wait();
            return taskMd5Checksum.Result;
        }

        /// <summary>
        /// Add a MD5 Checksum tot he Image Content
        /// </summary>
        /// <param name="imageContent"></param>
        public static async Task<string> GetMd5ChecksumAsync(IFileInfo fileInfo)
        {
            StringBuilder builder = new();

            var MD5Provider = MD5.Create();
            using (var stream = fileInfo.OpenRead())
            {
                byte[] buffer = await MD5Provider.ComputeHashAsync(stream);
                foreach (byte num in buffer)
                {
                    builder.Append(string.Format("{0:x2}", num));
                }
            }
            return builder.ToString();
        }
    }
}
