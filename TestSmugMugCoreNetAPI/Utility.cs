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
        album.BackprintingForPrints = "";
        album.BoutiquePackagingForOrders = SmugMug.Net.Data.Domain.Album.BoutiquePackaging.No;
        album.CanRank = true;
        album.CleanDisplay = true;
        album.ColorCorrection = SmugMug.Net.Data.Domain.Album.ColorCorrection.No;
        album.CommentsAllowed = true;
        album.Description = "This is a test album to verify my API";
        album.ExifAllowed = true;
        album.ExternalLinkAllowed = true;
        album.FamilyEditAllowed = true;
        album.FilenameDisplayWhenNoCaptions = true;
        album.FriendEditAllowed = true;
        album.GeographyMappingEnabled = true;
        album.HeaderDefaultIsSmugMug = true;
        album.HideOwner = false;
        album.InterceptShippingEnabled = SmugMug.Net.Data.Domain.Album.InterceptShipping.No;
        album.Keywords = "test;test2";
        album.NiceName = title + "NiceName";
        album.PackageBrandedOrdersEnabled = true;
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
        var albumData = (await service.GetAlbumList(Array.Empty<string>())).Where(x=>x.Title == title).ToArray();
        bool result = false;
        if (albumData.Length > 0)
            result = await service.DeleteAlbum(albumData[0].AlbumId);

        return result;
    }

    public async static Task<bool> RemoveArbitraryTestFamilies(SmugMug.Net.Core.SmugMugCore core, string nickname)
    {
        var service = new FamilyService(core);
        var data = (await service.GetFamilyList()).Where(x => x.NickName.Contains(nickname)).ToArray();
        foreach (var d in data)
        {
            _ = await service.RemoveFamily(d.NickName);
        }

        return true;
    }

    public async static Task<bool> RemoveArbitraryTestFriends(SmugMug.Net.Core.SmugMugCore core, string nickname)
    {
        var service = new FriendService(core);
        var data = (await service.GetFriendList()).Where(x => x.NickName.Contains(nickname)).ToArray();
        foreach (var d in data)
        {
            _ = await service.RemoveFriend(d.NickName);
        }

        return true;
    }
}
