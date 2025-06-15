using Microsoft.Extensions.Configuration;

namespace SmugMugCoreSync.Configuration
{
    public class UploaderConfig
    {
        public UploaderConfig(IConfiguration configuration)
        {
            this.UserName = Environment.GetEnvironmentVariable("SmugMugCoreSync::UserName") ?? configuration.GetSection("userName").Value ?? string.Empty; 
            this.UploadFolderPath = Environment.GetEnvironmentVariable("SmugMugCoreSync::UploadPath") ?? configuration.GetSection("uploadPath").Value ?? string.Empty;
        }

        /// <summary>
        /// Token from Smugmug representing the current user, authenticated to use the service
        /// </summary>
        public string UserName { get; }
        /// <summary>
        /// Private auth token for the user giving the permissions
        /// </summary>
        public string UploadFolderPath { get; }
    }
}
