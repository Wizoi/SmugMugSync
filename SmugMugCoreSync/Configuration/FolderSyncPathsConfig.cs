using Microsoft.Extensions.Configuration;
using System.Linq;

namespace SmugMugCoreSync.Configuration
{
    public class FolderSyncPathsConfig
    {
        public FolderSyncPathsConfig(IConfiguration configuration)
        {
            this.RootLocal = configuration.GetSection("rootLocal").Value ?? "";
            this.RootRemote = configuration.GetSection("rootRemote").Value ?? "";
            this.FilterFolderName = configuration.GetSection("filterFolderName").Value ?? "";
            this.FolderNamesToSkip = SyncAppSettings.ReadSectionArrayEntries(configuration.GetSection("folderNamesToSkip"));
            this.ExtensionsToSkip = SyncAppSettings.ReadSectionArrayEntries(configuration.GetSection("extensionsToSkip"));
        }

        public string RootLocal { get; }
        public string RootRemote { get; }
        public string FilterFolderName { get; }
        public string[] FolderNamesToSkip { get; }
        public string[] ExtensionsToSkip { get; }

    }
}
