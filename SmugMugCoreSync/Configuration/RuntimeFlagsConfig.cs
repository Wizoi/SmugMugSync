using Microsoft.Extensions.Configuration;

namespace SmugMugCoreSync.Configuration
{
    public enum OperationLevel
    {
        Normal,
        NoneLog,
        None
    }

    public class RuntimeFlagsConfig
    {
        public RuntimeFlagsConfig(IConfiguration configuration)
        {
            this.TargetCreate = ParseOperationLevelConfig(configuration.GetSection("targetCreate"));
            this.TargetDelete = ParseOperationLevelConfig(configuration.GetSection("targetDelete"));
            this.TargetUpdate = ParseOperationLevelConfig(configuration.GetSection("targetUpdate"));
            this.SourceVideoRedownload = ParseOperationLevelConfig(configuration.GetSection("sourceVideoRedownload"));
            this.IncludeVideos = bool.Parse(configuration.GetSection("includeVideos").Value ?? "false");
            this.ForceRefresh = bool.Parse(configuration.GetSection("forceRefresh").Value ?? "false");
            this.ImageUploadThrottle = int.Parse(configuration.GetSection("imageUploadThrottle").Value ?? "1");
        }

        private static OperationLevel ParseOperationLevelConfig(IConfigurationSection section)
        {
            string operationValue = section.Value ?? "None";
            if (Enum.TryParse<OperationLevel>(operationValue, true, out OperationLevel result))
                return result;
            else
                return OperationLevel.None;
        }

        public OperationLevel TargetCreate      { get; }
        public OperationLevel TargetDelete      { get; }
        public OperationLevel TargetUpdate      { get; }
        public OperationLevel SourceVideoRedownload { get; }
        public bool IncludeVideos       { get; }
        public bool ForceRefresh        { get; }
        public int ImageUploadThrottle  { get; }
    }
}
