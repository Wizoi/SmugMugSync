using System.Transactions;

namespace TestSmugMugCore20NetAPI;

public class Utility
{
    private static SmugMug.Net.Core20.SmugMugCore? core20;

    /// <summary>
    /// Authenticate and provide a smug mug core to test against
    /// </summary>
    /// <returns></returns>
    public static SmugMug.Net.Core20.SmugMugCore RetrieveSmugMugCore20()
    {
        if  (Utility.core20 == null)
        {
            var settings = new Configuration.KeySecretsConfig();
            var uploaderSettings = new Configuration.UploaderConfig();
            var manager = new SmugMug.Net.Core20.SmugMugCore(
                userAuthToken:  settings.UserAuthToken, userAuthSecret: settings.UserAuthSecret,
                apiKey: settings.ApiKey, apiSecret: settings.ApiSecret);
            manager.ConfigureApiDefaults(uploaderSettings.UserName, uploaderSettings.UploadFolderPath);
            manager.EnableRequestLogging = true;
            Utility.core20 = manager;
        }
        return Utility.core20;
    }
}
