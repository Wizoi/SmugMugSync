using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using RestSharp;
using SmugMug.Net.Core20;
using SmugMug.Net.Data20;

namespace SmugMug.Net.Service20
{
    public class AlbumImageService
    {
        public readonly string API_ALBUM_IMAGE_SEARCH = "/api/v2/album/{0}!images";
        public readonly string API_ALBUM_IMAGE_LOOKUP = "/api/v2/album/{0}/image/{1}";
        public readonly string API_ALBUM_IMAGE_DOWNLOAD = "/api/v2/album/{0}/image/{1}!download";

        private readonly Core20.SmugMugCore _core;

        internal AlbumImageService(Core20.SmugMugCore core)
        {
            _core = core;
        }

        // <summary>
        // Retrieve the information for an album
        // </summary>
        // <param name="albumId">The id for a specific album</param>
        // <param name="albumKey">The key for a specific album</param>
        // <returns></returns>
        public async virtual Task<Data20.AlbumImageDetail> GetImageDetail(string albumKey, string imageKey)
        {
            return await GetImageDetail(string.Format(API_ALBUM_IMAGE_LOOKUP, albumKey, imageKey));
        }

        // <summary>
        // Retrieve the information for an album
        // </summary>
        // <param name="albumId">The id for a specific album</param>
        // <param name="albumKey">The key for a specific album</param>
        // <returns></returns>
        public async virtual Task<Data20.AlbumImageDetail> GetImageDetail(string albumImageUri)
        {
            var request = new RestRequest(albumImageUri, Method.Get);
            var restResponse = await _core.QueryService(request);
            if (restResponse.IsSuccessful)
            {
                var jsonDoc = JsonDocument.Parse(restResponse.Content);
                var rawResponse = jsonDoc.RootElement.Deserialize<AlbumImageDetailResponse>();
                return rawResponse.Response.AlbumImage;
            }

            throw new Exception("Error loading album image: " + restResponse.ErrorMessage);
        }

        public async Task<Data20.AlbumImageDetail[]> GetAlbumImageListFull(string albumKey)
        {
            return await GetAlbumImageList(albumKey, [], 100);
        }

        public async Task<Data20.AlbumImageDetail[]> GetAlbumImageListShort(string albumKey)
        {
            return await GetAlbumImageList(albumKey,
                new string[] { "FileName", "Title", "Caption", "OriginalSize",  "Keywords", "AlbumKey", "ImageKey", "ArchivedMD5", "ArchivedUri" },
                300);
        }

        /// <summary>
        /// Retrieve a list of albums for a given user
        /// </summary>
        /// <param name="returnEmpty">Return empty albums, categories and subcategories</param>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <param name="fieldList">Extra fields to load in an album list (use data object fieldnames)</param>
        /// <returns></returns>
        private async Task<Data20.AlbumImageDetail[]> GetAlbumImageList(string albumKey, string[] fieldList, int pagingCount)
        {
            // Setup Request
            var initialRequest = new RestRequest(string.Format(API_ALBUM_IMAGE_SEARCH, albumKey), Method.Get);
            initialRequest.AddParameter("count", pagingCount, false);
            bool needsInitialRequest = true;

            // Add fields
            if (fieldList.Length > 0)
            {
                initialRequest.AddParameter("_filter", string.Join(",", fieldList), true);
                initialRequest.AddParameter("_filterurl", "", true);
            }

            // Make Call
            List<AlbumImageDetail> albumImageList = [];
            string nextPageLink = string.Empty;
            while (needsInitialRequest || (nextPageLink != null && !needsInitialRequest))
            {
                RestRequest request;
                if (needsInitialRequest)
                {
                    request = initialRequest;
                    needsInitialRequest = false;
                }
                else
                {
                    // Pull off the parameters
                    var nextPageParams = nextPageLink.Substring(nextPageLink.IndexOf("?") + 1);
                    var parametersCollection = HttpUtility.ParseQueryString(nextPageParams);
                    request = new RestRequest(string.Format(API_ALBUM_IMAGE_SEARCH, albumKey), Method.Get);
                    foreach (var param in parametersCollection.AllKeys)
                    {
                        request.Parameters.AddParameter(
                            RestSharp.Parameter.CreateParameter(param, parametersCollection[param], ParameterType.QueryString, true)
                        );
                    }
                }

                nextPageLink = null;
                var restResponse = await _core.QueryService(request);
                if (restResponse.IsSuccessful)
                {
                    var jsonDoc = JsonDocument.Parse(restResponse.Content);
                    var rawResponse = jsonDoc.RootElement.Deserialize<AlbumImageListResponse>();
                    if (rawResponse.Status == "fail")
                    {
                        throw new SmugMugException(request, restResponse.Content, rawResponse.Method, rawResponse.ErrorCode, rawResponse.ErrorMessage);
                    }

                    albumImageList.AddRange(rawResponse.Response.AlbumImages);
                    if (rawResponse.Response.Pages.NextPage != null)
                    {
                        nextPageLink = rawResponse.Response.Pages.NextPage;
                    }
                }
                else
                {
                    throw new Exception("Error loading album: " + restResponse.ErrorMessage);
                }
            }

            // Return Results
            return [.. albumImageList];
        }

        // <summary>
        // Deletes an image from an album
        // </summary>
        // <param name="albumId">The id for a specific album</param>
        // <param name="albumKey">The key for a specific album</param>
        // <returns></returns>
        public async virtual Task<string> DeleteImage(string imageUri)
        {
            var request = new RestRequest(imageUri, Method.Delete);

            var restResponse = await _core.QueryService(request);
            if (restResponse.IsSuccessful)
            {
                var jsonDoc = JsonDocument.Parse(restResponse.Content);
                var rawResponse = jsonDoc.RootElement.Deserialize<AlbumImageDeleteResponse>();
                if (rawResponse.Status == "fail")
                {
                    throw new SmugMugException(request, restResponse.Content, rawResponse.Method, rawResponse.ErrorCode, rawResponse.ErrorMessage);
                }
                return rawResponse.Response.Uri;
            }

            throw new Exception("Delete Failure: " + restResponse.ErrorMessage);
        }

        /// <summary>
        /// Create an album
        /// </summary>
        /// <param name="album">Album object to change settings on</param>
        /// <returns>New Album Album object with Key Information</returns>
        public async virtual Task<Data20.ImageDetail> UpdateAlbumImage(Data20.AlbumImageDetail albumImage)
        {
            AlbumImageUpdateRequest updateRequest = new AlbumImageUpdateRequest()
            {
                Title = albumImage.Title,
                Caption = albumImage.Caption,
                Keywords = albumImage.Keywords
            };

            string albumImageData = JsonSerializer.Serialize(updateRequest, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
            });

            var request = new RestRequest(albumImage.Uris.Image.Uri, Method.Patch);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("_verbosity", 1);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(updateRequest);

            var restResponse = await _core.PatchService(request);
            if (restResponse.IsSuccessful)
            {
                var jsonDoc = JsonDocument.Parse(restResponse.Content);
                var rawResponse = jsonDoc.RootElement.Deserialize<ImageDetailResponse>();
                if (rawResponse.Status == "fail")
                {
                    throw new SmugMugException(request, restResponse.Content, rawResponse.Method, rawResponse.ErrorCode, rawResponse.ErrorMessage);
                }
                return rawResponse.Response.Image;
            }

            throw new Exception($"Failed to create album: {albumImage.AlbumKey} / {albumImage.ImageKey} : {restResponse.ErrorMessage}");
        }

        /// <summary>
        /// Create an album
        /// </summary>
        /// <param name="album">Album object to change settings on</param>
        /// <returns>New Album Album object with Key Information</returns>
        public async virtual Task<bool> DownloadPrimaryImage(Data20.AlbumImageDetail albumImage, string filePath)
        {
            var request = new RestRequest(albumImage.ArchivedUri, Method.Get);
            byte[] fileData = await _core.DownloadData(request);
            File.WriteAllBytes(filePath, fileData);
            return true;
        }        
    }
}
