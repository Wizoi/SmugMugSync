namespace TestSmugMugCore20NetAPI.Configuration
{
    public class UploaderConfig
    {
        public UploaderConfig()
        {
            this.UserName = Environment.GetEnvironmentVariable("SmugMugCoreSync::UserName") ?? string.Empty; 
            this.UploadFolderPath = Environment.GetEnvironmentVariable("SmugMugCoreSync::UploadPath") ?? string.Empty;
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
