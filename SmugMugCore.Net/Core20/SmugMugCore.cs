using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using SmugMug.Net.Core;
using SmugMug.Net.Service;
using RestSharp;
using RestSharp.Authenticators;

namespace SmugMug.Net.Core20
{
    /// <summary>
    /// Base class which provides Authentication and core querying logic
    /// </summary>
    public class SmugMugCore
    {
        private readonly string _smugMugApiKey;
        private readonly string _smugMugSecret;
        private RestSharp.RestClient _client;
        private OAuthManager? _oauthManager = null;
        public bool EnableRequestLogging { get; set; }


        #region Properties for Services
        private readonly System.Collections.Generic.Dictionary<string, object> _serviceCatalog = new();


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

        /// <summary>
        /// Instantiate the object and authenticate
        /// </summary>
        /// <param name="userAuthToken"></param>
        /// <param name="userAuthSecret"></param>
        public SmugMugCore(string userAuthToken, string userAuthSecret, string  apiKey, string apiSecret) : this(apiKey, apiSecret)
        {
            Authenticate(userAuthToken, userAuthSecret);
        }

        /// <summary>
        /// Authenticate the user with their secret
        /// </summary>
        /// <param name="userAuthToken">User Authentication Token</param>
        /// <param name="userAuthSecret">User Authentication Secret</param>
        internal void Authenticate(string userAuthToken, string userAuthSecret)
        {
            var options = new RestClientOptions("https://api.smugmug.com")
            {
                Authenticator = OAuth1Authenticator.ForAccessToken(_smugMugApiKey, _smugMugSecret, userAuthToken, userAuthSecret) 
            };

            _client = new RestClient(options);
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
            RestResponse response = await _client.ExecuteAsync(request);

            // 3. Ensure a successful status code.
            if (!response.IsSuccessful)
            {
                throw new Exception($"Request failed with status code: {response.StatusCode}, Content: {response.Content}");
            }
            else
                return true;
        }
    }
}
