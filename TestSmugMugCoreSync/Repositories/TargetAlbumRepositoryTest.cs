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
    public async Task PopulateTargetAlbums_LoadFolderWithDash()
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
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();

        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "Expecting to find a folder");
    }

    [TestMethod]
    public async Task PopulateTargetAlbums_IgnoreFolderNoDate()
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
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();

        Assert.AreEqual(0, targetAlbumRepository.TargetAlbumCount(), "No Folders should be found");
    }

    [TestMethod]
    public async Task PopulateTargetAlbums_IgnoreTitleNotFilteredFolder()
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
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();

        Assert.AreEqual(0, targetAlbumRepository.TargetAlbumCount(), "No Folders should be found");
    }

    [TestMethod]
    public async Task PopulateTargetAlbums_ValidFilteredFolder()
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
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();

        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "Expecting to find a folder");
    }

    [TestMethod]
    public void VerifyLinkedFolder_LinkedFolderNotFound()
    {
        var inMemoryFolderSettings = new Dictionary<string, string?> {{"rootLocal", "A:\\FOODIRECTORY"}};
        var folderConfig = new FolderSyncPathsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryFolderSettings).Build());

        var inMemoryRuntimeSettings = new Dictionary<string, string?> {};
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        var smCoreMock = new Mock<SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});

        // Mocks to setup the Source Folder Repo
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        var xmlSystemMock = new Mock<XmlSystem>();
        var sourceFolderRepo = new Mock<SourceFolderRepository>(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);

        // Mocks to setup the Source Folders
        var directoryInfoMock1 = new Mock<IDirectoryInfo>();
        directoryInfoMock1.SetupGet(x => x.FullName).Returns("A:\\DIRECTORY1");
        fileSystemMock.Setup(x => x.File.Exists("A:\\DIRECTORY1\\.SMUGMUG.INI")).Returns(true);   
        fileSystemMock.Setup(x => x.File.ReadAllText("A:\\DIRECTORY1\\.SMUGMUG.INI")).Returns(
            "<smugMugSyncData><albumId>1</albumId><albumKey>someKey</albumKey></smugMugSyncData>");   
        var sourceDirDataMock = new Mock<SourceDirectoryData>(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock1.Object);
        sourceFolderRepo.Setup(x => x.RetrieveLinkedFolders()).Returns(new [] { sourceDirDataMock.Object });

        //
        // Actually setup the repository and run the test
        //
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);

        Assert.AreEqual(0, targetAlbumRepository.TargetAlbumCount(), "There are no loaded albums.");
        targetAlbumRepository.VerifyLinkedFolders(runtimeConfig, sourceFolderRepo.Object);
        Assert.AreEqual(0, targetAlbumRepository.TargetAlbumCount(), "There are no loaded albums.");
    }

    [TestMethod]
    public async Task VerifyLinkedFolder_UnlinkFolderNotMatching()
    {
        var inMemoryFolderSettings = new Dictionary<string, string?> {{"rootLocal", "A:\\FOODIRECTORY"}};
        var folderConfig = new FolderSyncPathsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryFolderSettings).Build());

        var inMemoryRuntimeSettings = new Dictionary<string, string?> {};
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        var smCoreMock = new Mock<SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});

        // Mocks to setup the Source Folder Repo
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        var xmlSystemMock = new Mock<XmlSystem>();
        var sourceFolderRepo = new Mock<SourceFolderRepository>(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);

        // Mocks to setup the Source Folders
        var directoryInfoMock1 = new Mock<IDirectoryInfo>();
        directoryInfoMock1.SetupGet(x => x.FullName).Returns("A:\\DIRECTORY1");
        fileSystemMock.Setup(x => x.File.Exists("A:\\DIRECTORY1\\.SMUGMUG.INI")).Returns(true);   
        fileSystemMock.Setup(x => x.File.ReadAllText("A:\\DIRECTORY1\\.SMUGMUG.INI")).Returns(
            "<smugMugSyncData><albumId>1</albumId><albumKey>someKey</albumKey></smugMugSyncData>");   
        var sourceDirDataMock = new Mock<SourceDirectoryData>(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock1.Object);
        sourceDirDataMock.Setup(x => x.UnlinkFromAlbum());
        sourceFolderRepo.Setup(x => x.RetrieveLinkedFolders()).Returns(new [] { sourceDirDataMock.Object });
        sourceFolderRepo.Setup(x => x.RemoveLinkedFolder(sourceDirDataMock.Object));

        // Album to Mock Load  
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestNonMatchKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        //
        // Actually setup the repository and run the test
        //
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();
        targetAlbumRepository.VerifyLinkedFolders(runtimeConfig, sourceFolderRepo.Object);
        
        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "There are no loaded albums.");
        sourceDirDataMock.Verify(x => x.UnlinkFromAlbum(), "Unlink From Album should have been called.");
        sourceFolderRepo.Verify(x => x.RemoveLinkedFolder(sourceDirDataMock.Object), "Source folder no longer linked.");
    }

    [TestMethod]
    public async Task SyncNewFolders_Validate()
    {
        var inMemoryFolderSettings = new Dictionary<string, string?> {{"rootLocal", "A:\\FOODIRECTORY"}};
        var folderConfig = new FolderSyncPathsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryFolderSettings).Build());

        var smCoreMock = new Mock<SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});

        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestNonMatchKey";

        // Mocks to setup the Source Folder Repo
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists("A:\\FOODIRECTORY")).Returns(true);   
        var xmlSystemMock = new Mock<XmlSystem>();
        var sourceFolderRepo = new Mock<SourceFolderRepository>(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);

        // Mocks to setup the Source Folders
        var directoryInfoMock1 = new Mock<IDirectoryInfo>();
        directoryInfoMock1.SetupGet(x => x.FullName).Returns("A:\\DIRECTORY1");
        fileSystemMock.Setup(x => x.File.Exists("A:\\DIRECTORY1\\.SMUGMUG.INI")).Returns(false);   
        var sourceDirDataMock = new Mock<SourceDirectoryData>(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock1.Object);
        sourceDirDataMock.Setup(x => x.LinkToAlbum(albDetail.AlbumId, albDetail.AlbumKey));
        sourceFolderRepo.Setup(x => x.RetrieveUnlinkedFolders()).Returns(new [] { sourceDirDataMock.Object });
        sourceFolderRepo.Setup(x => x.AddNewLinkedFolder(sourceDirDataMock.Object));

        // Album to Mock Load  
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        smAlbumServiceMock.Setup(x => x.CreateAlbum(It.IsAny<AlbumDetail>())).ReturnsAsync(albDetail);
        smAlbumServiceMock.Setup(x => x.GetAlbumDetail(albDetail.AlbumId, albDetail.AlbumKey))
            .ReturnsAsync(albDetail);
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        //
        // Setup the repository and run the test with NORMAL operation
        //
        var inMemoryRuntimeSettings = new Dictionary<string, string?> {{"targetCreate", "Normal"}};
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();
        _ = await targetAlbumRepository.SyncNewFolders(runtimeConfig, sourceFolderRepo.Object);
        
        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "There should be a loaded albums.");
        sourceDirDataMock.Verify(x => x.LinkToAlbum(albDetail.AlbumId, albDetail.AlbumKey), "Link to Album should have been called.");
        sourceFolderRepo.Verify(x => x.AddNewLinkedFolder(sourceDirDataMock.Object), "Source folder should be linked.");

        //
        // Setup the repository and run the test with NoneLog operation
        //
        inMemoryRuntimeSettings = new Dictionary<string, string?> {{"targetCreate", "NoneLog"}};
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();
        _ = await targetAlbumRepository.SyncNewFolders(runtimeConfig, sourceFolderRepo.Object);
        
        Assert.AreEqual(0, targetAlbumRepository.TargetAlbumCount(), "There should not be a loaded album.");

        //
        // Setup the repository and run the test with None operation
        //
        inMemoryRuntimeSettings = new Dictionary<string, string?> {{"targetCreate", "None"}};
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();
        _ = await targetAlbumRepository.SyncNewFolders(runtimeConfig, sourceFolderRepo.Object);
        
        Assert.AreEqual(0, targetAlbumRepository.TargetAlbumCount(), "There should not be a loaded album.");
    }

    [TestMethod]
    public async Task SyncExistingFolders_Validate()
    {
        var inMemoryFolderSettings = new Dictionary<string, string?> {{"rootLocal", "A:\\FOODIRECTORY"}};
        var folderConfig = new FolderSyncPathsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryFolderSettings).Build());

        var smCoreMock = new Mock<SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});

        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestNonMatchKey";

        // Mocks to setup the Source Folder Repo
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists("A:\\FOODIRECTORY")).Returns(true);   
        var xmlSystemMock = new Mock<XmlSystem>();
        var sourceFolderRepo = new Mock<SourceFolderRepository>(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);

        // Mocks to setup the Source Folders
        var directoryInfoMock1 = new Mock<IDirectoryInfo>();
        directoryInfoMock1.SetupGet(x => x.FullName).Returns("A:\\DIRECTORY1");
        fileSystemMock.Setup(x => x.File.Exists("A:\\DIRECTORY1\\.SMUGMUG.INI")).Returns(false);   
        var sourceDirDataMock = new Mock<SourceDirectoryData>(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock1.Object);
        sourceFolderRepo.Setup(x => x.RetrieveLinkedFolders()).Returns(new [] { sourceDirDataMock.Object });

        // Album to Mock Load  
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        smAlbumServiceMock.Setup(x => x.DeleteAlbum(It.IsAny<int>())).ReturnsAsync(true);
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        //
        // Setup the repository and run the test with NORMAL operation
        //
        var inMemoryRuntimeSettings = new Dictionary<string, string?> {{"targetDelete", "Normal"}};
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();
        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "There should be a loaded albums.");
        _ = await targetAlbumRepository.SyncExistingFolders(runtimeConfig, sourceFolderRepo.Object);
        Assert.AreEqual(0, targetAlbumRepository.TargetAlbumCount(), "Album should no longer be loaded.");
        smAlbumServiceMock.Verify(x => x.DeleteAlbum(albDetail.AlbumId), "Link to Album should have been called.");

        //
        // Setup the repository and run the test with NoneLog operation
        //
        inMemoryRuntimeSettings = new Dictionary<string, string?> {{"targetDelete", "NoneLog"}};
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();

        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "There should be a loaded albums.");
        _ = await targetAlbumRepository.SyncExistingFolders(runtimeConfig, sourceFolderRepo.Object);
        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "Album should still be loaded.");
        
        //
        // Setup the repository and run the test with None operation
        //
        inMemoryRuntimeSettings = new Dictionary<string, string?> {{"targetDelete", "None"}};
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();

        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "There should be a loaded albums.");
        _ = await targetAlbumRepository.SyncExistingFolders(runtimeConfig, sourceFolderRepo.Object);
        Assert.AreEqual(1, targetAlbumRepository.TargetAlbumCount(), "Album should still be loaded.");
    }

    [TestMethod]
    public async Task SyncFolderFiles_FolderNotFound()
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
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);


        // Mocks to setup the Source Folder Repo
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists("A:\\FOODIRECTORY")).Returns(true);   
        var xmlSystemMock = new Mock<XmlSystem>();
        var sourceFolderRepo = new Mock<SourceFolderRepository>(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);

        // Mocks to setup the Source Folders
        var directoryInfoMock1 = new Mock<IDirectoryInfo>();
        directoryInfoMock1.SetupGet(x => x.FullName).Returns("A:\\DIRECTORY1");
        fileSystemMock.Setup(x => x.File.Exists("A:\\DIRECTORY1\\.SMUGMUG.INI")).Returns(false);   
        var sourceDirDataMock = new Mock<SourceDirectoryData>(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock1.Object);
        sourceFolderRepo.Setup(x => x.RetrieveLinkedFolderByKey("TestKey")).Returns((SourceDirectoryData?)null);

        // START the test initiating the runtime config to use
        var inMemoryRuntimeSettings = new Dictionary<string, string?> {{"targetDelete", "Normal"}};
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();

        var actualStats = await targetAlbumRepository.SyncFolderFiles(runtimeFlags:runtimeConfig, sourceFolders:sourceFolderRepo.Object);
        Assert.AreEqual(1, actualStats.SkippedFolders, "Expecting to have a skipped folder");
        Assert.AreEqual(1, actualStats.ProcessedFolders, "Expecting to have one folder processed.");
    }

    [TestMethod]
    public async Task SyncFolderFiles_FolderWithNoFiles()
    {
        // Setup: For the folder config
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\2023 - TestTitle"},
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
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);
        var smImageServiceMock = new Mock<ImageService>(smCoreMock.Object);
        smImageServiceMock.Setup(x => x.GetAlbumImages(
            It.IsAny<string[]>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(albDetail);
        smCoreMock.Setup(x => x.ImageService).Returns(smImageServiceMock.Object);


        // Mocks to setup the Source Folder Repo
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists("A:\\2023 - TestTitle")).Returns(true);   
        var xmlSystemMock = new Mock<XmlSystem>();
        var sourceFolderRepo = new Mock<SourceFolderRepository>(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);

        // Mocks for the files we want to fake load
//        var fiMock = new Mock<IFileInfo>();
//        var fsiMock = new Mock<IFileSystemInfo>();
//        fsiMock.SetupGet(x => x.Name).Returns("Filename.JPG");
//        fileSystemMock.Setup(x => x.FileInfo.New("Filename.JPG")).Returns(fiMock.Object);   
//        var sourceMediaData = new SourceMediaData(fileSystemMock.Object, fsiMock.Object);

        // Mocks to setup the Source Folders
        var directoryInfoMock1 = new Mock<IDirectoryInfo>();
        directoryInfoMock1.SetupGet(x => x.FullName).Returns("A:\\DIRECTORY1");
        fileSystemMock.Setup(x => x.File.Exists("A:\\DIRECTORY1\\.SMUGMUG.INI")).Returns(false);   
        var sourceDirDataMock = new Mock<SourceDirectoryData>(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock1.Object);
        sourceFolderRepo.Setup(x => x.RetrieveLinkedFolderByKey("TestKey")).Returns(sourceDirDataMock.Object);
        sourceFolderRepo.Setup(x => x.LoadFolderMediaFiles(It.IsAny<SourceDirectoryData>())).Returns(Array.Empty<SourceMediaData>());

        // START the test initiating the runtime config to use
        var inMemoryRuntimeSettings = new Dictionary<string, string?> {{"targetDelete", "Normal"}};
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.PopulateTargetAlbums();

        var actualStats = await targetAlbumRepository.SyncFolderFiles(runtimeFlags:runtimeConfig, sourceFolders:sourceFolderRepo.Object);
        Assert.AreEqual(1, actualStats.ProcessedFolders, "Expecting to have one folder procoessed.");
        var actualFolderFileStats = actualStats.RetriveFolderFileStats()[0];
        Assert.AreEqual(0, actualFolderFileStats.DuplicateFiles, "Expecting to find no dupe files.");
        Assert.AreEqual(0, actualFolderFileStats.ResyncedFiles, "Expecting to find no resynced files.");
        Assert.AreEqual(0, actualFolderFileStats.AddedFiles, "Expecting to find no added files.");
        Assert.AreEqual(0, actualFolderFileStats.DeletedFiles, "Expecting to find no deleted files.");
        Assert.AreEqual("2023 - TestTitle", actualFolderFileStats.FolderName, "Expecting to find populated folder name.");
        
    }

}