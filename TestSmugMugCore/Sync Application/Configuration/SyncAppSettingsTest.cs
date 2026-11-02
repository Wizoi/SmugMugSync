using SmugMugCoreSync.Configuration;

namespace TestSmugMugCore.SyncApplication.Configuration;

/// <summary>
/// Tests for overriding the appsettings.json with the command line
/// </summary>
[TestClass()]
public class SyncAppSettingsTest
{
    [TestMethod()]
    public void FilterFolderName_FromAppSettings()
    {
        var expected = new SyncAppSettings().FolderSyncPaths.FilterFolderName;
        Assert.AreEqual(expected, new SyncAppSettings(["GENERATEKEYS"]).FolderSyncPaths.FilterFolderName);
    }

    [TestMethod()]
    public void FilterFolderName_OverriddenByCommandLine()
    {
        Assert.AreEqual("2026-05", new SyncAppSettings(["--filter", "2026-05"]).FolderSyncPaths.FilterFolderName);
        Assert.AreEqual("2026-05", new SyncAppSettings(["-f", "2026-05"]).FolderSyncPaths.FilterFolderName);
        Assert.AreEqual("2026-05", new SyncAppSettings(["--filter=2026-05"]).FolderSyncPaths.FilterFolderName);
        Assert.AreEqual("2026-05", new SyncAppSettings(["--folderSyncPaths:filterFolderName=2026-05"]).FolderSyncPaths.FilterFolderName);
    }
}
