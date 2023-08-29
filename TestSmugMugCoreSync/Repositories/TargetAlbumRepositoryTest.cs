using Microsoft.Extensions.Configuration;
using Moq;
using SmugMug.Net.Core;
using SmugMug.Net.Data;
using SmugMug.Net.Service;
using SmugMugCoreSync.Configuration;
using SmugMugCoreSync.Data;
using SmugMugCoreSync.Repositories;
using SmugMugCoreSync.Utility;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Xml.Linq;

namespace TestSmugMugCoreSync;

[TestClass]
public class TargetAlbumRepositoryTest
{
    [TestMethod]
    public void ConstructorTest()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\FOODIRECTORY"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var smCoreMock = new Mock<SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
    }

    [TestMethod]
    public void PopulateTargetAlbums_LoadFolderWithDash()
    {
        // Setup: For the folder config
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\FOODIRECTORY"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        // Setup: Populate an album to load
        var smCoreMock = new Mock<SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .Returns(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        targetAlbumRepository.PopulateTargetAlbums();

        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "Expecting to find a folder");
    }

    [TestMethod]
    public void PopulateTargetAlbums_IgnoreFolderNoDate()
    {
        // Setup: For the folder config
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\FOODIRECTORY"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        // Setup: Populate an album to load
        var smCoreMock = new Mock<SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        var albDetail = new AlbumDetail();
        albDetail.Title = "TestTitle";
        albDetail.AlbumKey = "TestKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .Returns(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        targetAlbumRepository.PopulateTargetAlbums();

        Assert.AreEqual(0, targetAlbumRepository.TargetAlbumCount(), "No Folders should be found");
    }

    [TestMethod]
    public void PopulateTargetAlbums_IgnoreTitleNotFilteredFolder()
    {
        // Setup: For the folder config
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\FOODIRECTORY"},
            {"filterFolderName", "2000"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        // Setup: Populate an album to load
        var smCoreMock = new Mock<SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .Returns(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        targetAlbumRepository.PopulateTargetAlbums();

        Assert.AreEqual(0, targetAlbumRepository.TargetAlbumCount(), "No Folders should be found");
    }

    [TestMethod]
    public void PopulateTargetAlbums_ValidFilteredFolder()
    {
        // Setup: For the folder config
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\FOODIRECTORY"},
            {"filterFolderName", "2023"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        // Setup: Populate an album to load
        var smCoreMock = new Mock<SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .Returns(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        targetAlbumRepository.PopulateTargetAlbums();

        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "Expecting to find a folder");
    }


}