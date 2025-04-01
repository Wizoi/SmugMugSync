using System.Transactions;

namespace TestSmugMugCoreNetAPI;

public class Utility
{
    private static SmugMug.Net.Core.SmugMugCore? core = null;

    /// <summary>
    /// Authenticate and provide a smug mug core to test against
    /// </summary>
    /// <returns></returns>
    public static SmugMug.Net.Core.SmugMugCore RetrieveSmugMugCore()
    {
        if  (Utility.core == null)
        {
            var settings = new Configuration.KeySecretsConfig();
            var manager = new SmugMug.Net.Core.SmugMugCore(
                userAuthToken:  settings.UserAuthToken, userAuthSecret: settings.UserAuthSecret,
                apiKey: settings.ApiKey, apiSecret: settings.ApiSecret);
            manager.EnableRequestLogging = true;
            Utility.core = manager;
        }
        return Utility.core;
    }

    /// <summary>
    /// Create an album for testing
    /// </summary>
    /// <param name="core"></param>
    /// <returns></returns>
    public async static Task<SmugMug.Net.Data.AlbumDetail> CreateArbitraryTestAlbum(SmugMug.Net.Core.SmugMugCore core, string title)
    {
        // Remove it first if it exists from a prior run to clean up
        _ = await RemoveArbitraryTestAlbum(core, title);

        AlbumService service = new AlbumService(core); 
        var album = new AlbumDetail();
        album.Title = title;
        album.CanRank = true;
        album.CommentsAllowed = true;
        album.ExifAllowed = true;
        album.GeographyMappingEnabled = true;
        album.NiceName = title + "NiceName";
        album.PublicDisplay = true;
        var newAlbum = await service.CreateAlbum(album);
        return newAlbum;
    }

    /// <summary>
    /// Remove an arbitrary test album
    /// </summary>
    /// <param name="core"></param>
    /// <param name="album"></param>
    /// <returns></returns>
    public async static Task<bool> RemoveArbitraryTestAlbum(SmugMug.Net.Core.SmugMugCore core, string title)
    {
        var service = new AlbumService(core);
        var albumData = (await service.GetAlbumList([])).Where(x=>x.Title == title).ToArray();
        bool result = false;
        if (albumData.Length > 0)
            result = await service.DeleteAlbum(albumData[0].AlbumId);

        return result;
    }

}
