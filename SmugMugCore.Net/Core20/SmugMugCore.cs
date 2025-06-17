using System.Net;
using SmugMug.Net.Service20;
using RestSharp;
using RestSharp.Authenticators;
using SmugMug.Net.Data;

namespace SmugMug.Net.Core20
{
    /// <summary>
    /// Base class which provides Authentication and core querying logic
    /// </summary>
    public class SmugMugCore
    {
        private readonly string _smugMugApiKey;
        private readonly string _smugMugSecret;
        private string _smugMugUserToken;
        private string _smugMugUserSecret;
        //        private RestSharp.RestClient _client;
        //        private RestSharp.RestClientOptions _authOptions;
        private string _userName;
        private string _uploadFolderPath;
        public bool EnableRequestLogging { get; set; }


        #region Properties for Services
        private readonly System.Collections.Generic.Dictionary<string, object> _serviceCatalog = new();

        public virtual AlbumService AlbumService
        {
            get
            {
                string keyName = "AlbumService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.TryAdd(keyName, new AlbumService(this, _userName, _uploadFolderPath)); }
                return (AlbumService)_serviceCatalog[keyName];
            }
        }

        public virtual AlbumImageService AlbumImageService
        {
            get
            {
                string keyName = "AlbumImageService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.TryAdd(keyName, new AlbumImageService(this)); }
                return (AlbumImageService)_serviceCatalog[keyName];
            }
        }

        public virtual ImageUploaderService ImageUploaderService
        {
            get
            {
                string keyName = "ImageUploaderService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.TryAdd(keyName, new ImageUploaderService(this)); }
                return (ImageUploaderService)_serviceCatalog[keyName];
            }
        }

        public virtual ContentMetadataService ContentMetadataService
        {
            get
            {
                string keyName = "ContentMetadataService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.TryAdd(keyName, new ContentMetadataService()); }
                return (ContentMetadataService)_serviceCatalog[keyName];
            }
        }

        #endregion Properties for Services

        /// <summary>
        /// Instantiate an unauthenticated Core library
        /// </summary>
        public SmugMugCore(string apiKey, string apiSecret)
        {
            this.EnableRequestLogging = System.Diagnostics.Debugger.IsAttached;
            this._smugMugSecret = apiSecret;
            this._smugMugApiKey = apiKey;
        }

        public void ConfigureApiDefaults(string userName, string defaultUploadFolder)
        {
            this._userName = userName;
            this._uploadFolderPath = defaultUploadFolder;
        }

        /// <summary>
        /// Instantiate the object and authenticate
        /// </summary>
        /// <param name="userAuthToken"></param>
        /// <param name="userAuthSecret"></param>
        public SmugMugCore(string userAuthToken, string userAuthSecret, string apiKey, string apiSecret) : this(apiKey, apiSecret)
        {
            _smugMugUserToken = userAuthToken;
            _smugMugUserSecret = userAuthSecret;
        }

        /// <summary>
        /// Authenticate the user with their secret
        /// </summary>
        /// <param name="userAuthToken">User Authentication Token</param>
        /// <param name="userAuthSecret">User Authentication Secret</param>
        internal RestClient Authenticate()
        {
            var authOptions = new RestClientOptions("https://api.smugmug.com")
            {
                Authenticator = OAuth1Authenticator.ForAccessToken(_smugMugApiKey, _smugMugSecret, _smugMugUserToken, _smugMugUserSecret),
                AutomaticDecompression = DecompressionMethods.All,
                PreAuthenticate = true
            };

            return new RestClient(authOptions);
        }

        /// <summary>
        /// Ping the service, throw an exception if there are problems
        /// </summary>
        /// <returns>True if service is pingable</returns>
        public async Task<bool> PingService()
        {
            // 1. Construct the request
            var request = new RestRequest("/api/v2!authuser", Method.Get);

            // 2. Execute the request and get the response.
            var client = this.Authenticate();
            RestResponse response = await client.ExecuteAsync(request);

            // 3. Ensure a successful status code.
            if (!response.IsSuccessful)
            {
                throw new Exception($"Request failed with status code: {response.StatusCode}, Content: {response.Content}");
            }
            else
                return true;
        }

        public async Task<RestResponse> QueryService(RestRequest request)
        {
            RestResponse response;
            using (var client = this.Authenticate())
            {
                response = await client.ExecuteAsync(request);
            }

            // 3. Ensure a successful status code.
            if (!response.IsSuccessful)
            {
                throw new Exception($"Query Request failed with status code: {response.StatusCode}, Content: {response.Content}");
            }
            else
                return response;
        }

        public async Task<RestResponse> PostService(RestRequest request)
        {
            RestResponse response;
            using (var client = this.Authenticate())
            {
                response = await client.ExecutePostAsync(request);
            }

            // 3. Ensure a successful status code.
            if (!response.IsSuccessful)
            {
                throw new Exception($"Post Request failed with status code: {response.StatusCode}, Content: {response.Content}, Error: {response.ErrorMessage}");
            }
            else
                return response;
        }

        public async Task<RestResponse> PatchService(RestRequest request)
        {
            RestResponse response;
            using (var client = this.Authenticate())
            {
                response = await client.ExecutePatchAsync(request);
            }

            // 3. Ensure a successful status code.
            if (!response.IsSuccessful)
            {
                 throw new Exception($"Patch Request failed with status code: {response.StatusCode}, Content: {response.Content}, Error: {response.ErrorMessage}");
            }
            else
                return response;
        }
    }
}
