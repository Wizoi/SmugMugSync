using Microsoft.Extensions.Configuration;

namespace SmugMugCoreSync.Configuration
{
    public class KeySecretsConfig
    {
        public KeySecretsConfig(IConfiguration configuration)
        {
            this.UserAuthToken = Environment.GetEnvironmentVariable("SmugMugCoreSync::UserAuthToken") ?? configuration.GetSection("userAuthToken").Value ?? string.Empty; 
            this.UserAuthSecret = Environment.GetEnvironmentVariable("SmugMugCoreSync::UserAuthSecret") ?? configuration.GetSection("userAuthSecret").Value ?? string.Empty;
            this.ApiKey = Environment.GetEnvironmentVariable("SmugMugCoreSync::ApiKey") ?? configuration.GetSection("apiKey").Value ?? string.Empty;
            this.ApiSecret = Environment.GetEnvironmentVariable("SmugMugCoreSync::ApiSecret") ?? configuration.GetSection("apiSecret").Value ?? string.Empty;
        }

        /// <summary>
        /// Token from Smugmug representing the current user, authenticated to use the service
        /// </summary>
        public string UserAuthToken { get; }
        /// <summary>
        /// Private auth token for the user giving the permissions
        /// </summary>
        public string UserAuthSecret { get; }
        /// <summary>
        /// Key from smugmug represenging the Oauth Smugmugsync service access
        /// </summary>
        public string ApiKey { get; }
        /// <summary>
        /// API Secret indicating this app will make calls on behalf of the smugmugsync service.
        /// </summary>
        public string ApiSecret { get; }
    }
}
