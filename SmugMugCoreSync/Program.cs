using SmugMug.Net.Core;
using SmugMug.Net.Data;
using SmugMugCoreSync.Configuration;
using SmugMugCoreSync.Repositories;
using SmugMugCoreSync.Utility;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Formats.Asn1.AsnWriter;

/*
 *  Initialize Logging and core App
 */
internal class Program
{
    private static async Task Main(string[] args)
    {
        var appSettings = new SyncAppSettings();
        var keySettings = appSettings.KeySecrets;
        var appLogger = new AppLogger(appSettings.Logging);
        appLogger.SetupAppLog();

        var smCore = new SmugMugCore(
            userAuthToken: keySettings.UserAuthToken, userAuthSecret: keySettings.UserAuthSecret,
            apiKey: keySettings.ApiKey, apiSecret: keySettings.ApiSecret);

        if (args.Any() && args[0] == "GENERATEKEYS")
        {
            _ = await GenerateUserAccessTokens(keySettings.ApiKey, keySettings.ApiSecret);
        }
        else
        {
            /* Load Source Files */
            var sourceFolders = new SourceFolderRepository(
                fileSystem: new FileSystem(),
                xmlSystem: new XmlSystem(),
                folderConfig: appSettings.FolderSyncPaths);
            sourceFolders.PopulateSourceFoldersAndFiles();

            /* Load Remote Albums */
            var remoteAlbums = new TargetAlbumRepository(core: smCore, folderConfig: appSettings.FolderSyncPaths);
            _ = await remoteAlbums.PopulateTargetAlbums();
            remoteAlbums.VerifyLinkedFolders(appSettings.RuntimeFlags, sourceFolders);
            _ = await remoteAlbums.SyncNewFolders(appSettings.RuntimeFlags, sourceFolders);
            _ = await remoteAlbums.SyncExistingFolders(appSettings.RuntimeFlags, sourceFolders);
            _ = await remoteAlbums.SyncFolderFiles(appSettings.RuntimeFlags, sourceFolders);
        }
    }

    /// <summary>
    /// Authenticates against SmugMug to get the user's access key
    /// </summary>
    static async Task<bool> GenerateUserAccessTokens(string smugMugApiKey, string smugMugSecret)
    {
        Console.WriteLine("Create new instance of OAuth Manager");
        var manager = new OAuthManager(smugMugApiKey, smugMugSecret);

        Console.WriteLine("Aquire a request token from smugmug");
        var requestToken = await manager.AcquireRequestToken(new Uri("https://api.smugmug.com/services/oauth/getRequestToken.mg"), HttpMethod.Get, new());

        Console.WriteLine("Browser should open, please authorize request token to continue.");
        Process.Start(new ProcessStartInfo($"https://api.smugmug.com/services/oauth/authorize.mg?oauth_token={requestToken.Token}&Access=Full&Permissions=Modify") { UseShellExecute = true });

        Console.WriteLine("Please enter you credentials into the browser before continuing");
        Console.WriteLine("Press enter to continue...");
        Console.ReadLine();

        // Recreate the Oauth Manager with the new token    
        manager = new OAuthManager(smugMugApiKey, smugMugSecret, requestToken.Token, requestToken.TokenSecret);
        var accessToken = await manager.AcquireAccessToken(new Uri("https://api.smugmug.com/services/oauth/getAccessToken.mg"), HttpMethod.Get, requestToken.TokenSecret, new());

        Console.WriteLine(string.Format("User Access Token: {0}", accessToken.Token));
        Console.WriteLine(string.Format("User Access Secret: {0}", accessToken.TokenSecret));
        Console.WriteLine("Press enter to continue.");
        Console.ReadLine();

        Console.WriteLine("Verifying access to SmugMug with New Keys...");
        var smCore = new SmugMugCore(
            userAuthToken: accessToken.Token, userAuthSecret: accessToken.TokenSecret,
            apiKey: smugMugApiKey, apiSecret: smugMugSecret);
        var checkAccess = await smCore.PingService();
        Console.WriteLine($" -> Is Valid? {checkAccess.ToString()}");
        Console.ReadLine();

        return true;
    }
}