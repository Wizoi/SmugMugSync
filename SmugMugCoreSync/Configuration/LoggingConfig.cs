using Microsoft.Extensions.Configuration;

namespace SmugMugCoreSync.Configuration
{
    public class LoggingConfig
    {
        public LoggingConfig(IConfiguration configuration)
        {
            this.LogFileEditorPath = configuration.GetSection("logFileEditorPath").Value ?? "";
            this.FilePurgeCount = int.Parse(configuration.GetSection("filePurgeCount").Value ?? "15");
            this.OpenLogOnFailure = bool.Parse(configuration.GetSection("openLogOnFailure").Value ?? "false");
        }

        public string LogFileEditorPath { get; }
        public int FilePurgeCount { get; }
        public bool OpenLogOnFailure { get; }
    }
}
