namespace TestSmugMugCoreNetAPI.Configuration
{
    public class KeySecretsConfig
    {
        public KeySecretsConfig()
        {
            this.UserAuthToken = Environment.GetEnvironmentVariable("SmugMugCoreSync::UserAuthToken") ?? string.Empty; 
            this.UserAuthSecret = Environment.GetEnvironmentVariable("SmugMugCoreSync::UserAuthSecret") ?? string.Empty;
            this.ApiKey = Environment.GetEnvironmentVariable("SmugMugCoreSync::ApiKey") ?? string.Empty;
            this.ApiSecret = Environment.GetEnvironmentVariable("SmugMugCoreSync::ApiSecret") ?? string.Empty;
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
