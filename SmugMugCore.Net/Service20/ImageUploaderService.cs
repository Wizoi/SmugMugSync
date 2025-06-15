using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using SixLabors.ImageSharp;
using SmugMug.Net.Core20;
using SmugMug.Net.Data20;

namespace SmugMug.Net.Service20
{
    public class ImageUploaderService
    {
        private readonly Core20.SmugMugCore _core;
        private readonly string Version = "2.0";
        public readonly string API_UPLOADER_ENDPOINT = "https://upload.smugmug.com/";

        public ImageUploaderService(Core20.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Uploads the image
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="imageId"></param>
        /// <param name="imageMetadata"></param>
        public async virtual Task<string> UploadUpdatedImage(string albumUri, string imageUri, Data20.FileMetaContent imageMetadata)
        {
            if (imageMetadata.FileInfo == null)
                throw new ApplicationException("Error - Image with no filename detected.");

            if (string.IsNullOrEmpty(imageMetadata.MD5Checksum))
                imageMetadata.MD5Checksum = await GetMd5Checksum(imageMetadata.FileInfo);

            var fileInfo = imageMetadata.FileInfo;
            var request = new RestRequest(API_UPLOADER_ENDPOINT, Method.Post);
            request.AlwaysSingleFileAsContent = true;
            request.AddHeader("Content-MD5", imageMetadata.MD5Checksum);
            request.AddHeader("Content-Length", fileInfo.Length.ToString());
            request.AddHeader("Content-Type", "none");

            // Required Params
            request.AddHeader("X-Smug-Version", Version);
            request.AddHeader("X-Smug-ResponseType", "JSON");
            request.AddHeader("X-Smug-AlbumUri", albumUri);

            // Additional Params
            if (imageMetadata.Title != null)
                request.AddHeader("X-Smug-Caption", imageMetadata.Title);
            else if (imageMetadata.Caption != null)
                request.AddHeader("X-Smug-Caption", imageMetadata.Caption);
            else
                request.AddHeader("X-Smug-Caption", "");
            request.AddHeader("X-Smug-FileName", imageMetadata.FileInfo.Name);
            if (imageUri != null && imageUri.Length > 0)
                request.AddHeader("X-Smug-ImageID", imageUri);
            if (imageMetadata.Keywords != null)
                request.AddHeader("X-Smug-Keywords", string.Join(";", imageMetadata.Keywords));
            if (imageMetadata.GeoAltitude != 0)
                request.AddHeader("X-Smug-Altitude", imageMetadata.GeoAltitude.ToString());
            if (imageMetadata.GeoLatitude != 0)
                request.AddHeader("X-Smug-Latitude", imageMetadata.GeoLatitude.ToString());
            if (imageMetadata.GeoLongitude != 0)
                request.AddHeader("X-Smug-Longitude", imageMetadata.GeoLongitude.ToString());

            request.AddFile("upload", fileInfo.FullName);
            var restResponse = await _core.PostService(request);
            if (restResponse.IsSuccessful)
            {
                var jsonDoc = JsonDocument.Parse(restResponse.Content);
                var rawResponse = jsonDoc.RootElement.Deserialize<AlbumImageUploadResponse>();
                if (rawResponse.Status == "fail")
                {
                    throw new SmugMugException(request, restResponse.Content, rawResponse.Method, rawResponse.ErrorCode, rawResponse.ErrorMessage);
                }
                return rawResponse.Image.AlbumImageUri;
            }

            throw new Exception("Error uploading image: " + restResponse.ErrorMessage);
        }

        /// <summary>
        /// Add a MD5 Checksum tot he Image Content
        /// </summary>
        /// <param name="imageContent"></param>
        public static async Task<string> GetMd5Checksum(IFileInfo fileInfo)
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