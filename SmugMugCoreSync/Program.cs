using RestSharp;
using RestSharp.Authenticators;
using SmugMugCore.Net.Core20;
using SmugMugCore.Net.Data20;
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

        var smCore = new SmugMugCore.Net.Core20.SmugMugCore(
            userAuthToken: keySettings.UserAuthToken, userAuthSecret: keySettings.UserAuthSecret,
            apiKey: keySettings.ApiKey, apiSecret: keySettings.ApiSecret,
            userName: appSettings.UploaderSettings.UserName,
            defaultUploadFolder: appSettings.UploaderSettings.UploadFolderPath);

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
        Console.WriteLine("1. Acquire a request token from SmugMug to request authentication");
        var forRequestAuthOptions = new RestClientOptions("https://secure.smugmug.com/services/oauth/1.0a")
        {
            Authenticator = OAuth1Authenticator.ForRequestToken(smugMugApiKey, smugMugSecret, "oob"),
            AutomaticDecompression = System.Net.DecompressionMethods.All,
            PreAuthenticate = true
            
        };
        var forRequestClient = new RestClient(forRequestAuthOptions);
        var forRequestRestRequest = new RestRequest("getRequestToken", Method.Post);
        var forRequestResponse = forRequestClient.Execute(forRequestRestRequest);

        Console.WriteLine("3. Retrieve user token and secret");
        var forRequestResponseData = System.Web.HttpUtility.ParseQueryString(forRequestResponse.Content);
        string initialRequestToken = forRequestResponseData["oauth_token"];
        string initialRequestTokenSecret = forRequestResponseData["oauth_token_secret"];

        Console.WriteLine("4. Have user authorize token");
        Console.WriteLine("Browser should open, please authorize request token to continue.");
        string authorizationUrl = $"https://api.smugmug.com/services/oauth/1.0a/authorize?oauth_token={initialRequestToken}&Access=Full&Permissions=Modify";
        Process.Start(new ProcessStartInfo(authorizationUrl) { UseShellExecute = true });
        Console.WriteLine("Please enter you credentials into the browser before continuing");
        Console.Write("Press enter your verification code and press enter: ");
        string verifier = Console.ReadLine();

        // Recreate the Oauth Manager with the new token    
        var forAccessAuthOptions = new RestClientOptions("https://api.smugmug.com/services/oauth/1.0a")
        {
            Authenticator = OAuth1Authenticator.ForAccessToken(smugMugApiKey, smugMugSecret, initialRequestToken, initialRequestTokenSecret),
            PreAuthenticate = true
        };
        var forAccessClient = new RestClient(forAccessAuthOptions);
        var forAccessRestRequest = new RestRequest("getAccessToken", Method.Get);
        forAccessRestRequest.AddParameter("oauth_verifier", verifier);
        var forAccessResponse = forAccessClient.Execute(forAccessRestRequest);
        var forAccessResponseData = System.Web.HttpUtility.ParseQueryString(forAccessResponse.Content);
        string finalRequestToken = forAccessResponseData["oauth_token"];
        string finalRequestTokenSecret = forAccessResponseData["oauth_token_secret"];


        Console.WriteLine($"User Access Token: {finalRequestToken}");
        Console.WriteLine($"User Access Secret: {finalRequestTokenSecret}");
        Console.WriteLine("Press enter to test this with an a Ping API.");
        Console.ReadLine();

        Console.WriteLine("5. Verifying access to SmugMug with New Keys...");
        var smCore = new SmugMugCore.Net.Core20.SmugMugCore(
            userAuthToken: finalRequestToken, userAuthSecret: finalRequestTokenSecret,
            apiKey: smugMugApiKey, apiSecret: smugMugSecret, string.Empty, string.Empty);
        var checkAccess = await smCore.PingService();
        Console.WriteLine($" -> Is successful? {checkAccess.ToString()}");
        Console.ReadLine();

        return true;
    }
}