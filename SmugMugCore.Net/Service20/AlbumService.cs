using System.Text.Json;
using System.Web;
using RestSharp;
using SmugMugCore.Net.Core20;
using SmugMugCore.Net.Data20;

namespace SmugMugCore.Net.Service20
{
    public class AlbumService
    {
        public readonly string API_ALBUM_SEARCH = "/api/v2/album!search";
        public readonly string API_ALBUM_LOOKUP = "/api/v2/album/{0}";
        public readonly string API_ALBUM_NEW_LOCATION = "/api/v2/folder/user/{0}/{1}!albums";
        public readonly string ROOT_SCOPE = "/api/v2/user/{0}";


        private readonly Core20.SmugMugCore _core;
        private readonly string _userName;
        private readonly string _uploadPath;

        internal AlbumService(Core20.SmugMugCore core, string userName, string uploadPath)
        {
            _core = core;
            _userName = userName;
            _uploadPath = uploadPath;
        }

        /// <summary>
        /// Retrieve a list of albums for a given user with a field list.  Will return empty albums, and ignore the nickname, and album passwords
        /// </summary>
        /// <param name="fieldList">Extra fields to load in an album list (use data object fieldnames)</param>
        /// <returns></returns>
        public async virtual Task<Data20.AlbumDetail[]> GetAlbumListFull()
        {
            return await GetAlbumList([], "", 200);
        }

        /// <summary>
        /// Retrieves album keys and titles for a given case insensitive search text
        /// </summary>
        /// <param name="searchText">case insensitive search term</param>
        /// <returns></returns>
        public async virtual Task<Data20.AlbumDetail[]> GetAlbumListNamesOnly(string searchText)
        {
            return await GetAlbumList(new[] { "AlbumKey", "Name" }, searchText, 1000);
        }

        /// <summary>
        /// Retrieve a list of albums for a given user
        /// </summary>
        /// <param name="returnEmpty">Return empty albums, categories and subcategories</param>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <param name="sitePassword">The site password for a specific user</param>
        /// <param name="fieldList">Extra fields to load in an album list (use data object fieldnames)</param>
        /// <returns></returns>
        private async Task<Data20.AlbumDetail[]> GetAlbumList(string[] fieldList, string searchText, int pagingCount)
        {
            // Setup Request
            var initialRequest = new RestRequest(API_ALBUM_SEARCH, Method.Get);
            initialRequest.AddParameter("Scope", string.Format(ROOT_SCOPE, _userName), true);
            initialRequest.AddParameter("count", pagingCount, false);
            initialRequest.AddParameter("Text", searchText, true);
            bool needsInitialRequest = true;

            // Add fields
            if (fieldList.Length > 0)
            {
                initialRequest.AddParameter("_filter", string.Join(",", fieldList), true);
                initialRequest.AddParameter("_filterurl", "", true);
            }

            // Make Call
            List<AlbumDetail> albumList = [];
            string nextPageLink = string.Empty;
            while (needsInitialRequest || (!string.IsNullOrEmpty(nextPageLink) && !needsInitialRequest))
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
                    request = new RestRequest(API_ALBUM_SEARCH, Method.Get);
                    foreach (var param in parametersCollection.AllKeys)
                    {
                        request.Parameters.AddParameter(
                            RestSharp.Parameter.CreateParameter(param, parametersCollection[param], ParameterType.QueryString, true)
                        );
                    }
                }

                nextPageLink = string.Empty;
                var restResponse = await _core.QueryService(request);
                if (restResponse.IsSuccessful)
                {
                    var jsonDoc = JsonDocument.Parse(restResponse.Content);
                    var rawResponse = jsonDoc.RootElement.Deserialize<AlbumListResponse>();
                    if (rawResponse.Status == "fail")
                    {
                        throw new SmugMugException(request, restResponse.Content, rawResponse.Method, rawResponse.ErrorCode, rawResponse.ErrorMessage);
                    }

                    albumList.AddRange(rawResponse.Response.AlbumDetail);
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
            return [.. albumList];
        }

        // <summary>
        // Retrieve the information for an album
        // </summary>
        // <param name="albumId">The id for a specific album</param>
        // <param name="albumKey">The key for a specific album</param>
        // <returns></returns>
        public async virtual Task<Data20.AlbumDetail> GetAlbumDetail(string albumKey)
        {
            var request = new RestRequest(string.Format(API_ALBUM_LOOKUP, albumKey), Method.Get);
            var restResponse = await _core.QueryService(request);
            if (restResponse.IsSuccessful)
            {
                var jsonDoc = JsonDocument.Parse(restResponse.Content);
                var rawResponse = jsonDoc.RootElement.Deserialize<AlbumDetailResponse>();
                if (rawResponse.Status == "fail")
                {
                    throw new SmugMugException(request, restResponse.Content, rawResponse.Method, rawResponse.ErrorCode, rawResponse.ErrorMessage);
                }
                return rawResponse.Response.AlbumDetail;
            }

            throw new Exception("Error loading album: " + restResponse.ErrorMessage);
        }

        /// <summary>
        /// Create an album
        /// </summary>
        /// <param name="album">Album object to change settings on</param>
        /// <returns>New Album Album object with Key Information</returns>
        public async virtual Task<Data20.AlbumDetail> CreateAlbum(Data20.AlbumDetail album)
        {
            // NiceName is required, make sure it is setup
            if (album.NiceName == null || album.NiceName.Length == 0)
            {
                album.NiceName = string.Concat(album.Name.Where(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c))).Replace(" ", "-");
            }

            string albumData = JsonSerializer.Serialize(album, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
            });

            string albumUploadLocation = string.Format(API_ALBUM_NEW_LOCATION, _userName, _uploadPath);
            var request = new RestRequest(albumUploadLocation, Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("_verbosity", 1);
            request.AddHeader("Content-Type", "application/json");
            request.AddBody(albumData);

            var restResponse = await _core.QueryService(request);
            if (restResponse.IsSuccessful)
            {
                var jsonDoc = JsonDocument.Parse(restResponse.Content);
                AlbumCreateResponse rawResponse = jsonDoc.RootElement.Deserialize<AlbumCreateResponse>();
                if (rawResponse.Status == "fail")
                {
                    throw new SmugMugException(request, restResponse.Content, rawResponse.Method, rawResponse.ErrorCode, rawResponse.ErrorMessage);
                }
                return rawResponse.Response.AlbumDetail;
            }

            throw new Exception("Failed to create album: " + album.Name + ", " + restResponse.ErrorMessage);
        }
        
        // <summary>
        // Retrieve the information for an album
        // </summary>
        // <param name="albumId">The id for a specific album</param>
        // <param name="albumKey">The key for a specific album</param>
        // <returns></returns>
        public async virtual Task<string> DeleteAlbum(AlbumDetail album)
        {
            var request = new RestRequest(string.Format(API_ALBUM_LOOKUP, album.AlbumKey), Method.Delete);
            var restResponse = await _core.QueryService(request);
            if (restResponse.IsSuccessful)
            {
                var jsonDoc = JsonDocument.Parse(restResponse.Content);
                var rawResponse = jsonDoc.RootElement.Deserialize<AlbumDeleteResponse>();
                if (rawResponse.Status == "fail")
                {
                    throw new SmugMugException(request, restResponse.Content, rawResponse.Method, rawResponse.ErrorCode, rawResponse.ErrorMessage);
                }
                return rawResponse.Response.Uri;
            }

            throw new Exception("Delete Failure: " + restResponse.ErrorMessage);
        }        
    }
}
