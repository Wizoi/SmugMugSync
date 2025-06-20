using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RestSharp;
using SmugMugCore.Net.Core20;
using SmugMugCore.Net.Data20;

namespace SmugMugCore.Net.Service20
{
    public class ImageUploaderService
    {
        private readonly Core20.SmugMugCore? _core;
        private readonly string Version = "2.0";
        public readonly string API_UPLOADER_ENDPOINT = "https://upload.smugmug.com/";

        public ImageUploaderService() : this(null) { }

        public ImageUploaderService(Core20.SmugMugCore? core)
        {
            _core = core;
        }

        /// <summary>
        /// Upload a new Image to an Album
        /// </summary>
        /// <param name="albumKey"></param>
        /// <param name="imageMetadata"></param>
        /// <returns></returns>
        public async virtual Task<string> UploadAlbumImage(string albumKey, Data20.FileMetaContent imageMetadata)
        {
            var album = await _core.AlbumService.GetAlbumDetail(albumKey);
            return await this.UploadAlbumImage(new UriMetadata() { Uri = album.Uri }, null, imageMetadata);
        }

        /// <summary>
        /// Upload an updated image, Look up the required album and image URIs from the main objects before uploading. 
        /// </summary>
        /// <param name="albumKey"></param>
        /// <param name="imageKey"></param>
        /// <param name="imageMetadata"></param>
        /// <returns></returns>
        public async virtual Task<string> UploadAlbumImage(string albumKey, string imageKey, int imageKeySerial, Data20.FileMetaContent imageMetadata)
        {
            var albumImage = await _core.AlbumImageService.GetImageDetail(albumKey, imageKey, imageKeySerial);
            return await this.UploadAlbumImage(albumImage.Uris.Album, albumImage.Uris.Image, imageMetadata);
        }

        /// <summary>
        /// Uploads the image
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="imageId"></param>
        /// <param name="imageMetadata"></param>
        private async Task<string> UploadAlbumImage(UriMetadata albumUri, UriMetadata? imageUri, Data20.FileMetaContent imageMetadata)
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
            request.AddHeader("X-Smug-AlbumUri", albumUri.Uri);
            if (imageUri != null)
                request.AddHeader("X-Smug-ImageUri", imageUri.Uri);

            // Additional Params
            if (imageMetadata.Title != null)
                request.AddHeader("X-Smug-Title", imageMetadata.Title);
            else
                request.AddHeader("X-Smug-Title", "");
            if (imageMetadata.Caption != null)
                request.AddHeader("X-Smug-Caption", imageMetadata.Caption);
            else
                request.AddHeader("X-Smug-Caption", "");
            request.AddHeader("X-Smug-FileName", imageMetadata.FileInfo.Name);
            if (imageMetadata.Keywords != null)
                request.AddHeader("X-Smug-Keywords", string.Join(";", imageMetadata.Keywords));
            if (imageMetadata.GeoAltitude != 0)
                request.AddHeader("X-Smug-Altitude", imageMetadata.GeoAltitude.ToString());
            if (imageMetadata.GeoLatitude != 0)
                request.AddHeader("X-Smug-Latitude", imageMetadata.GeoLatitude.ToString());
            if (imageMetadata.GeoLongitude != 0)
                request.AddHeader("X-Smug-Longitude", imageMetadata.GeoLongitude.ToString());

            // Setting a very large timeout as some videos can take an hour or so to download
            request.Timeout = TimeSpan.FromHours(6);

            request.AddFile(
                name: "file", // This is the name of the parameter on the server-side for the file
                getFile: () => 
                {
                    // Streaming the file during the upload to save memory for the big videos
                    return fileInfo.OpenRead();
                },
                fileName: fileInfo.Name,
                contentType: RestSharp.ContentType.Binary,
                options: null
            );       
            
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