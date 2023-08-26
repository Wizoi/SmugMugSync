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

        public virtual string RootLocal { get; }
        public virtual string RootRemote { get; }
        public virtual string FilterFolderName { get; }
        public virtual string[] FolderNamesToSkip { get; }
        public virtual string[] ExtensionsToSkip { get; }

    }
}
