using Microsoft.Extensions.Configuration;

namespace SmugMugCoreSync.Configuration
{
    public class SyncAppSettings
    {
        public SyncAppSettings()
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.dev.json", optional: true, reloadOnChange: true);

            var rootConfig = builder.Build();

            // Populate the required config
            this.FolderSyncPaths = new(rootConfig.GetSection("folderSyncPaths"));
            this.KeySecrets = new(rootConfig.GetSection("keySecrets"));
            this.Logging = new(rootConfig.GetSection("logging"));
            this.RuntimeFlags = new(rootConfig.GetSection("runtimeFlags"));
            this.UploaderSettings = new(rootConfig.GetSection("uploaderConfig"));
        }

        public FolderSyncPathsConfig FolderSyncPaths { get; }
        public KeySecretsConfig KeySecrets { get; }
        public LoggingConfig Logging { get; }
        public RuntimeFlagsConfig RuntimeFlags { get; }
        public UploaderConfig UploaderSettings { get; }


        public static string[] ReadSectionArrayEntries(IConfiguration configSection)
        {
            var arrayConfigList = configSection.AsEnumerable();
            if (arrayConfigList.Any())
            {
                return arrayConfigList.ToDictionary(
                    setting => setting.Key.ToString(),
                    setting => setting.Value?.ToString() ?? "").Where(t => t.Value.Any()).Select(y => y.Value).ToArray();
            }
            else
                return [];

        }

    }
}
