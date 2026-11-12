using System.Text.Json;
using System.Web;
using RestSharp;
using SmugMugCore.Net.Core20;
using SmugMugCore.Net.Data20;

namespace SmugMugCore.Net.Service20
{
    public class AlbumService(Core20.SmugMugCore? core, string userName, string uploadPath)
    {
        public readonly string API_USER_ALBUMS = "/api/v2/user/{0}!albums";
        public readonly string API_ALBUM_LOOKUP = "/api/v2/album/{0}";
        public readonly string API_ALBUM_NEW_LOCATION = "/api/v2/folder/user/{0}/{1}!albums";


        public AlbumService() : this(null, "", "") { }

        /// <summary>
        /// Retrieve all of the albums for the user, with all of their fields
        /// </summary>
        /// <returns></returns>
        public async virtual Task<Data20.AlbumDetail[]> GetAlbumListFull()
        {
            return await GetAlbumList([]);
        }

        /// <summary>
        /// Retrieves album keys and names, for the albums with a name containing the text (case insensitive, empty for all albums)
        /// </summary>
        /// <param name="searchText">case insensitive text the album name contains</param>
        /// <returns></returns>
        public async virtual Task<Data20.AlbumDetail[]> GetAlbumListNamesOnly(string searchText)
        {
            var albums = await GetAlbumList(["AlbumKey", "Name"]);
            return albums.Where(x => x.Name != null && x.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        /// <summary>
        /// Retrieve all of the albums for the user (user!albums). Album search (album!search) is not used, as it is a text search
        /// which leaves out albums (ie: 32 of the 96 albums named 199).
        /// SmugMug's paging is by position (start/count), so albums can be repeated or skipped when the order shifts between pages.
        /// The albums are listed again until all of SmugMug's total is found.
        /// </summary>
        /// <param name="fieldList">Extra fields to load in an album list (use data object fieldnames)</param>
        /// <returns></returns>
        /// <exception cref="Exception">if not every album is listed, as the sync would create the missing albums again</exception>
        private async Task<Data20.AlbumDetail[]> GetAlbumList(string[] fieldList)
        {
            const int maxPasses = 3;
            var albums = new Dictionary<string, AlbumDetail>();
            int total = 0;
            for (int pass = 1; pass <= maxPasses; pass++)
            {
                total = await AddAlbumListPages(fieldList, albums);
                if (albums.Count >= total)
                    return [.. albums.Values];
            }

            throw new Exception($"Only {albums.Count} of {total} albums were listed by SmugMug after {maxPasses} attempts.");
        }

        /// <summary>
        /// Page through the user's albums, adding each to the albums found
        /// </summary>
        /// <returns>SmugMug's total number of albums</returns>
        private async Task<int> AddAlbumListPages(string[] fieldList, Dictionary<string, AlbumDetail> albums)
        {
            // Setup Request (the page size is limited to 100 by SmugMug)
            var initialRequest = new RestRequest(string.Format(API_USER_ALBUMS, userName), Method.Get);
            initialRequest.AddParameter("count", 100, false);
            initialRequest.AddParameter("Order", "Date Added (Descending)", true);

            // Add fields
            if (fieldList.Length > 0)
            {
                initialRequest.AddParameter("_filter", string.Join(",", fieldList), true);
                initialRequest.AddParameter("_filteruri", "", true);
            }

            // Make Call
            int total = 0;
            RestRequest? request = initialRequest;
            while (request != null)
            {
                var restResponse = await core.QueryService(request);
                if (!restResponse.IsSuccessful)
                    throw new Exception("Error loading album: " + restResponse.ErrorMessage);

                var jsonDoc = JsonDocument.Parse(restResponse.Content);
                var rawResponse = jsonDoc.RootElement.Deserialize<AlbumListResponse>();
                if (rawResponse.Status == "fail")
                {
                    throw new SmugMugException(request, restResponse.Content, rawResponse.Method, rawResponse.ErrorCode, rawResponse.ErrorMessage);
                }

                foreach (var album in rawResponse.Response.AlbumDetail ?? [])
                {
                    if (album.AlbumKey != null)
                        albums.TryAdd(album.AlbumKey, album);
                }

                total = rawResponse.Response.Pages?.Total ?? 0;
                string? nextPageLink = rawResponse.Response.Pages?.NextPage;
                request = null;
                if (!string.IsNullOrEmpty(nextPageLink))
                {
                    // Pass the parameters of the next page decoded, so they are not encoded twice (ie: Order has spaces)
                    var nextPageParams = HttpUtility.ParseQueryString(nextPageLink.Substring(nextPageLink.IndexOf('?') + 1));
                    request = new RestRequest(nextPageLink.Substring(0, nextPageLink.IndexOf('?')), Method.Get);
                    foreach (var param in nextPageParams.AllKeys)
                    {
                        request.Parameters.AddParameter(
                            RestSharp.Parameter.CreateParameter(param, nextPageParams[param], ParameterType.QueryString, true)
                        );
                    }
                }
            }

            return total;
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
            var restResponse = await core.QueryService(request);
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
            album.Name = CleanAlbumName(album.Name);

            // NiceName is required, make sure it is setup
            if (album.UrlName == null || album.UrlName.Length == 0)
            {
                album.UrlName = string.Concat(album.Name.Where(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c) || (c == '-'))).Replace(" ", "-").Replace("--", "-");
            }

            string albumData = JsonSerializer.Serialize(album, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
            });

            string albumUploadLocation = string.Format(API_ALBUM_NEW_LOCATION, userName, uploadPath);
            var request = new RestRequest(albumUploadLocation, Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("_verbosity", 1);
            request.AddHeader("Content-Type", "application/json");
            request.AddBody(albumData);

            var restResponse = await core.PostService(request);
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
        
        /// <summary>
        /// Clean a folder name to use as an album name. Apostrophes (ie: ' and the Unicode ’) in a folder name failed the sync, so they are removed.
        /// </summary>
        public static string? CleanAlbumName(string? name)
        {
            if (name == null)
                return null;

            return string.Concat(name.Where(c => c != '\'' && c != '’' && c != '‘' && c != 'ʼ'));
        }

        // <summary>
        // Retrieve the information for an album
        // </summary>
        // <param name="albumId">The id for a specific album</param>
        // <param name="albumKey">The key for a specific album</param>
        // <returns></returns>
        public async virtual Task<bool> DeleteAlbum(AlbumDetail album)
        {
            var request = new RestRequest(string.Format(API_ALBUM_LOOKUP, album.AlbumKey), Method.Delete);
            var restResponse = await core.QueryService(request);
            if (restResponse.IsSuccessful)
            {
                var jsonDoc = JsonDocument.Parse(restResponse.Content);
                var rawResponse = jsonDoc.RootElement.Deserialize<AlbumDeleteResponse>();
                if (rawResponse.Status == "fail")
                {
                    throw new SmugMugException(request, restResponse.Content, rawResponse.Method, rawResponse.ErrorCode, rawResponse.ErrorMessage);
                }
                return true;
            }

            throw new Exception("Delete Failure: " + restResponse.ErrorMessage);
        }        
    }
}
