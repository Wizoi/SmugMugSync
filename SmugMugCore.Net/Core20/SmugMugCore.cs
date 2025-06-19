using System.Net;
using SmugMugCore.Net.Service20;
using RestSharp;
using RestSharp.Authenticators;

namespace SmugMugCore.Net.Core20
{
    /// <summary>
    /// Base class which provides Authentication and core querying logic
    /// </summary>
    public class SmugMugCore(
        string userAuthToken,
        string userAuthSecret,
        string apiKey,
        string apiSecret,
        string userName,
        string defaultUploadFolder)
    {
        public SmugMugCore() : this("", "", "", "", "", "") { }

        public bool EnableRequestLogging { get; set; } = System.Diagnostics.Debugger.IsAttached;

        #region Properties for Services
        private readonly System.Collections.Generic.Dictionary<string, object> _serviceCatalog = new();

        public virtual AlbumService AlbumService
        {
            get
            {
                string keyName = "AlbumService";
                if (!_serviceCatalog.ContainsKey(keyName)) { _serviceCatalog.TryAdd(keyName, new AlbumService(this, userName, defaultUploadFolder)); }
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
        /// Authenticate to the service with the user's API keys and tokens
        /// Returns an authenticated RestClient
        /// </summary>
        internal RestClient Authenticate()
        {
            var authOptions = new RestClientOptions("https://api.smugmug.com")
            {
                Authenticator = OAuth1Authenticator.ForAccessToken(apiKey, apiSecret, userAuthToken, userAuthSecret),
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

        /// <summary>
        ///  Queries the SmugMug Service with a Rest Request
        /// </summary>
        /// <param name="request">RestRequest object</param>
        /// <returns>RestResponse object with JSON response</returns>
        /// <exception cref="Exception">if there is a response failure</exception>
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

        /// <summary>
        /// Posts a payload to the SmugMug Service
        /// </summary>
        /// <param name="request">RestRequest object</param>
        /// <returns>RestResponse object with JSON response</returns>
        /// <exception cref="Exception">if there is a response failure</exception>
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

        /// <summary>
        /// Patches/Updates the SmugMug Service
        /// </summary>
        /// <param name="request">RestRequest object</param>
        /// <returns>RestResponse object with JSON response</returns>
        /// <exception cref="Exception">if there is a response failure</exception>
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

        /// <summary>
        /// Downloads data from the SmugMug Service
        /// </summary>
        /// <param name="request">RestRequest object</param>
        /// <returns>binary that was downloaded</returns>
        /// <exception cref="Exception">if there is a failure</exception>
        public async Task<byte[]> DownloadData(RestRequest request)
        {
            using (var client = this.Authenticate())
            {
                byte[] dataDownloaded = await client.DownloadDataAsync(request) ?? [];
                return dataDownloaded;
            }

            throw new Exception($"File download failed: {request.Resource}");
        }
    }
}
