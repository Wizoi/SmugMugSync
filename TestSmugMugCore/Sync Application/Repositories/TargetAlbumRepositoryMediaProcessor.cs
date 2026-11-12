using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using SmugMugCore.Net.Core20;
using SmugMugCore.Net.Data20;
using SmugMugCore.Net.Service20;
using SmugMugCoreSync.Configuration;
using SmugMugCoreSync.Data;
using SmugMugCoreSync.Repositories;
using SmugMugCoreSync.Utility;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Net.Mail;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Xml.Linq;

namespace TestSmugMugCore.SyncApplication.RepositoryTests;

[TestClass]
public class TargetAlbumRepositoryMediaProcessor
{
    [TestMethod]
    public async Task ProcessExistingRemoteMedia_Scenario1_Redownload_FileLength0()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\2023 - TestTitle"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);
        var smCoreMock = new Mock<SmugMugCore.Net.Core20.SmugMugCore>(new object[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
        var uploadThrottler = new SemaphoreSlim(1);

        // Setup the Target Image (ImageDetail)
        var targetImage = new AlbumImageDetail() { AlbumKey = "TestKey", ImageKey = "SomeKey1", FileName = "TestFile.JPG" };
        var smImageServiceMock = new Mock<AlbumImageService>(smCoreMock.Object);
        smImageServiceMock.Setup(x => x.GetImageDetail(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(targetImage);
        smImageServiceMock.Setup(x => x.GetAlbumImageListShort(It.IsAny<string>())).ReturnsAsync([targetImage]);
        smImageServiceMock.Setup(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>())).ReturnsAsync(true);            
        smCoreMock.Setup(x => x.AlbumImageService).Returns(smImageServiceMock.Object);

        // Setup the Target Album (AlbumDetail) / ALBUMSERVICE
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object, "", "");
        var albDetail = new AlbumDetail();
        albDetail.Name = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumListNamesOnly(It.IsAny<string>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);


        // Setup the Source Metadata (FileMetaContent) / CONTENTMETADATASERVICE
        var targetMetadata = new FileMetaContent();
        var smContentServiceMock = new Mock<ContentMetadataService>();
        smContentServiceMock.Setup(x => x.DiscoverMetadata(It.IsAny<string>()))
            .ReturnsAsync(targetMetadata);
        smCoreMock.Setup(x => x.ContentMetadataService).Returns(smContentServiceMock.Object);

        // Setup the Source Image (SourceMediaData)
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists("A:\\2023 - TestTitle")).Returns(true);   
        var fiMock = new Mock<IFileInfo>();
        var fsiMock = new Mock<IFileSystemInfo>();
        fsiMock.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock.SetupGet(x => x.FullName).Returns("FILENAME.JPG");
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);   
        var sourceMediaData = new SourceMediaData(fileSystemMock.Object, fsiMock.Object);

        // Scenario Setup - Using a file with  0 Length
        fiMock.SetupGet(x => x.Length).Returns(0);

        ////////////////////////////////////////////////
        // Perform the Image test AS NORMAL
        var inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"sourceRedownload", "Normal"}
        };
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        var result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaData, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>()));
        smImageServiceMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image test AS NONELOG
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"sourceRedownload", "NoneLog"}
        };
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaData, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>()), Times.Never());
        smImageServiceMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with IncludeVideos = FALSE
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"sourceRedownload", "Normal"},
            {"includeVideos", "false"},
        };
        targetMetadata.IsVideo = true;

        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaData, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>()), Times.Never());
        smImageServiceMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with IncludeVideos = TRUE
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"sourceRedownload", "Normal"},
            {"includeVideos", "true"},
        };
        targetMetadata.IsVideo = true;

        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaData, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>()));
        smImageServiceMock.Invocations.Clear();

    }

    [TestMethod]
    public async Task ProcessExistingRemoteMedia_Scenario2_RefreshRemoteImage()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\2023 - TestTitle"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);
        var smCoreMock = new Mock<SmugMugCore.Net.Core20.SmugMugCore>(new object[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
        var uploadThrottler = new SemaphoreSlim(1);

        // Setup the Target Image (ImageDetail)
        var targetImage = new AlbumImageDetail() { AlbumKey = "TestKey", ImageKey = "SomeKey1", FileName = "TestFile.JPG" };
        var smImageServiceMock = new Mock<AlbumImageService>(smCoreMock.Object);
        smImageServiceMock.Setup(x => x.GetImageDetail(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(targetImage);
        smImageServiceMock.Setup(x => x.GetAlbumImageListShort(It.IsAny<string>())).ReturnsAsync([targetImage]);
        smImageServiceMock.Setup(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>())).ReturnsAsync(true);            
        smCoreMock.Setup(x => x.AlbumImageService).Returns(smImageServiceMock.Object);

        // Setup the Target Album (AlbumDetail) / ALBUMSERVICE
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object, "", "");
        var albDetail = new AlbumDetail();
        albDetail.Name = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumListNamesOnly(It.IsAny<string>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        // Setup the Source Metadata (FileMetaContent) / CONTENTMETADATASERVICE
        var targetMetadata = new FileMetaContent();
        var smContentServiceMock = new Mock<ContentMetadataService>();
        smContentServiceMock.Setup(x => x.DiscoverMetadata(It.IsAny<string>()))
            .ReturnsAsync(targetMetadata);
        smCoreMock.Setup(x => x.ContentMetadataService).Returns(smContentServiceMock.Object);

        // Setup the Source Image (SourceMediaData)
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists("A:\\2023 - TestTitle")).Returns(true);   
        var fiMock = new Mock<IFileInfo>();
        var fsiMock = new Mock<IFileSystemInfo>();
        fsiMock.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock.SetupGet(x => x.FullName).Returns("FILENAME.JPG");
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);   
        var sourceMediaDataMock = new Mock<SourceMediaData>(fileSystemMock.Object, fsiMock.Object);

        // Setup the ImageUploaderService
        var imageUploaded = new ImageUploaderService();
        var smImageUploaderSvcMock = new Mock<ImageUploaderService>(smCoreMock.Object);
        smImageUploaderSvcMock.Setup(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FileMetaContent>())).ReturnsAsync(It.IsAny<string>());
        smCoreMock.Setup(x => x.ImageUploaderService).Returns(smImageUploaderSvcMock.Object);

        // Scenario Setup - Setup the length and size fields to be different, but the source is not 0
        fiMock.SetupGet(x => x.Length).Returns(1);
        targetImage.ArchivedSize = 2;
        // Scenario Setup - Setup the checksum to be  different
        sourceMediaDataMock.Setup(x => x.LoadMd5Checksum()).ReturnsAsync("ChecksumData");
        targetImage.ArchivedMD5 = "ChecksumOrigData";
        // Scenario Setup - Mark the date on the file and the local file to be the same
        fiMock.SetupGet(x => x.LastWriteTime).Returns(DateTime.Parse("2020-01-01"));
        targetImage.LastUpdated = DateTime.Parse("2020-01-01");

        ////////////////////////////////////////////////
        // Perform the test AS NORMAL
        var inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetUpdate", "Normal"}
        };
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        var result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageUploaderSvcMock.Verify(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FileMetaContent>()));
        smImageUploaderSvcMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the test AS NONELOG
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetUpdate", "NoneLog"}
        };
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageUploaderSvcMock.Verify(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FileMetaContent>()), Times.Never());
        smImageUploaderSvcMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with IncludeVideos = FALSE
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetUpdate", "Normal"},
            {"includeVideos", "false"},
        };
        targetMetadata.IsVideo = true;

        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageUploaderSvcMock.Verify(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FileMetaContent>()), Times.Never());
        smImageUploaderSvcMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with IncludeVideos = TRUE
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetUpdate", "Normal"},
            {"includeVideos", "true"},
        };
        targetMetadata.IsVideo = true;

        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageUploaderSvcMock.Verify(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FileMetaContent>()), Times.Never());
        smImageUploaderSvcMock.Invocations.Clear();
    }

    [TestMethod]
    public async Task ProcessExistingRemoteMedia_Scenario2_RedownloadImage()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\2023 - TestTitle"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);
        var smCoreMock = new Mock<SmugMugCore.Net.Core20.SmugMugCore>(new object[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
        var uploadThrottler = new SemaphoreSlim(1);

        // Setup the Target Image (ImageDetail)
        var targetImage = new AlbumImageDetail() { AlbumKey = "TestKey", ImageKey = "SomeKey1", FileName = "TestFile.JPG" };
        var smImageServiceMock = new Mock<AlbumImageService>(smCoreMock.Object);
        smImageServiceMock.Setup(x => x.GetImageDetail(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(targetImage);
        smImageServiceMock.Setup(x => x.GetAlbumImageListShort(It.IsAny<string>())).ReturnsAsync([targetImage]);
        smImageServiceMock.Setup(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>())).ReturnsAsync(true);            
        smCoreMock.Setup(x => x.AlbumImageService).Returns(smImageServiceMock.Object);

        // Setup the Target Album (AlbumDetail) / ALBUMSERVICE
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object, "", "");
        var albDetail = new AlbumDetail();
        albDetail.Name = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumListNamesOnly(It.IsAny<string>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        // Setup the Source Metadata (FileMetaContent) / CONTENTMETADATASERVICE
        var targetMetadata = new FileMetaContent();
        var smContentServiceMock = new Mock<ContentMetadataService>();
        smContentServiceMock.Setup(x => x.DiscoverMetadata(It.IsAny<string>()))
            .ReturnsAsync(targetMetadata);
        smCoreMock.Setup(x => x.ContentMetadataService).Returns(smContentServiceMock.Object);

        // Setup the Source Image (SourceMediaData)
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists("A:\\2023 - TestTitle")).Returns(true);   
        var fiMock = new Mock<IFileInfo>();
        var fsiMock = new Mock<IFileSystemInfo>();
        fsiMock.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock.SetupGet(x => x.FullName).Returns("FILENAME.JPG");
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);   
        var sourceMediaDataMock = new Mock<SourceMediaData>(fileSystemMock.Object, fsiMock.Object);

        // Setup the ImageUploaderService
        var imageUploaded = new ImageUploaderService();
        var smImageUploaderSvcMock = new Mock<ImageUploaderService>(smCoreMock.Object);
        smImageUploaderSvcMock.Setup(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FileMetaContent>())).ReturnsAsync(It.IsAny<string>());
        smCoreMock.Setup(x => x.ImageUploaderService).Returns(smImageUploaderSvcMock.Object);

        // Scenario Setup - Setup the length and size fields to be different, but the source is not 0
        fiMock.SetupGet(x => x.Length).Returns(1);
        targetImage.ArchivedSize = 2;
        // Scenario Setup - Setup the checksum to be  different
        sourceMediaDataMock.Setup(x => x.LoadMd5Checksum()).ReturnsAsync("ChecksumData");
        targetImage.ArchivedMD5 = "ChecksumOrigData";
        // Scenario Setup - Source file is image dated before the target date (implies it was reprocessed by smugmug)
        targetImage.LastUpdated = DateTime.Parse("2020-01-02");
        fiMock.SetupGet(x => x.LastWriteTime).Returns(DateTime.Parse("2020-01-01"));

        ////////////////////////////////////////////////
        // Perform the test AS NORMAL
        var inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"sourceRedownload", "Normal"},
        };
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        var result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>()));
        smImageServiceMock.Invocations.Clear();    

        ////////////////////////////////////////////////
        // Perform the test AS NONELOG
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"sourceRedownload", "NoneLog"},
        };
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>()), Times.Never());
        smImageServiceMock.Invocations.Clear();    

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with IncludeVideos = FALSE
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"sourceRedownload", "Normal"},
            {"includeVideos", "false"},
        };
        targetMetadata.IsVideo = true;

        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>()), Times.Never());
        smImageServiceMock.Invocations.Clear();    

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with IncludeVideos = TRUE, Regardless of setting, downloads of videos should NOT happen
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"sourceRedownload", "Normal"},
            {"includeVideos", "true"},
        };
        targetMetadata.IsVideo = true;

        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>()), Times.Never());
        smImageServiceMock.Invocations.Clear();    
    }

    [TestMethod]
    public async Task ProcessExistingRemoteMedia_Scenario2_UpdateMetadata()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\2023 - TestTitle"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);
        var smCoreMock = new Mock<SmugMugCore.Net.Core20.SmugMugCore>(new object[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
        var uploadThrottler = new SemaphoreSlim(1);

        // Setup the Target Image (ImageDetail)
        var targetImage = new AlbumImageDetail() { AlbumKey = "TestKey", ImageKey = "SomeKey1", FileName = "TestFile.JPG" };
        var smImageServiceMock = new Mock<AlbumImageService>(smCoreMock.Object);
        smImageServiceMock.Setup(x => x.GetImageDetail(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(targetImage);
        smImageServiceMock.Setup(x => x.GetAlbumImageListShort(It.IsAny<string>())).ReturnsAsync([targetImage]);
        smImageServiceMock.Setup(x => x.DownloadPrimaryImage(targetImage, It.IsAny<string>())).ReturnsAsync(true);            
        smCoreMock.Setup(x => x.AlbumImageService).Returns(smImageServiceMock.Object);

        // Setup the Target Album (AlbumDetail) / ALBUMSERVICE
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object, "", "");
        var albDetail = new AlbumDetail();
        albDetail.Name = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumListNamesOnly(It.IsAny<string>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        // Setup the Source Metadata (FileMetaContent) / CONTENTMETADATASERVICE
        var targetMetadata = new FileMetaContent();
        var smContentServiceMock = new Mock<ContentMetadataService>();
        smContentServiceMock.Setup(x => x.DiscoverMetadata(It.IsAny<string>()))
            .ReturnsAsync(targetMetadata);
        smCoreMock.Setup(x => x.ContentMetadataService).Returns(smContentServiceMock.Object);

        // Setup the Source Image (SourceMediaData)
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists("A:\\2023 - TestTitle")).Returns(true);   
        var fiMock = new Mock<IFileInfo>();
        var fsiMock = new Mock<IFileSystemInfo>();
        fsiMock.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock.SetupGet(x => x.FullName).Returns("FILENAME.JPG");
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);   
        var sourceMediaDataMock = new Mock<SourceMediaData>(fileSystemMock.Object, fsiMock.Object);

        // Setup the ImageUploaderService
        var imageUploaded = new ImageUploaderService();
        var smImageUploaderSvcMock = new Mock<ImageUploaderService>(smCoreMock.Object);
        smImageUploaderSvcMock.Setup(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FileMetaContent>())).ReturnsAsync(It.IsAny<string>());
        smCoreMock.Setup(x => x.ImageUploaderService).Returns(smImageUploaderSvcMock.Object);

        // Scenario Setup - Size, Dates and checksum match
        fiMock.SetupGet(x => x.Length).Returns(1);
        targetImage.ArchivedSize = 1;
        sourceMediaDataMock.Setup(x => x.LoadMd5Checksum()).ReturnsAsync("ChecksumData");
        targetImage.ArchivedMD5 = "ChecksumData";
        targetImage.LastUpdated = DateTime.Parse("2020-01-01");
        fiMock.SetupGet(x => x.LastWriteTime).Returns(DateTime.Parse("2020-01-01"));

        // Scenario Setup - Make keywords different
        targetImage.Keywords = "TestKeyword";

        ////////////////////////////////////////////////
        // Perform the test AS NORMAL
        var inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetUpdate", "Normal"},
        };
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        var result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.UpdateAlbumImage(targetImage));
        smImageServiceMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the test AS NONE
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetUpdate", "NoneLog"},
        };
        targetImage.Keywords = "TestKeyword";
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.UpdateAlbumImage(targetImage), Times.Never());
        smImageServiceMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with IncludeVideos = FALSE
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetUpdate", "Normal"},
            {"includeVideos", "false"},
        };
        targetMetadata.IsVideo = true;

        targetImage.Keywords = "TestKeyword";
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.UpdateAlbumImage(targetImage), Times.Never());
        smImageServiceMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with IncludeVideos = TRUE
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetUpdate", "Normal"},
            {"includeVideos", "true"},
        };
        targetMetadata.IsVideo = true;

        targetImage.Keywords = "TestKeyword";
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.UpdateAlbumImage(targetImage));
        smImageServiceMock.Invocations.Clear();
    }

    [TestMethod]
    public async Task ProcessNewRemoteMedia_BaseMedia()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\2023 - TestTitle"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);
        var smCoreMock = new Mock<SmugMugCore.Net.Core20.SmugMugCore>(new object[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
        var uploadThrottler = new SemaphoreSlim(1);

        // Setup the Target Album (AlbumDetail) / ALBUMSERVICE
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object, "", "");
        var albDetail = new AlbumDetail();
        albDetail.Name = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumListNamesOnly(It.IsAny<string>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        // Setup the Source Metadata (FileMetaContent) / CONTENTMETADATASERVICE
        var targetMetadata = new FileMetaContent();
        var smContentServiceMock = new Mock<ContentMetadataService>();
        smContentServiceMock.Setup(x => x.DiscoverMetadata(It.IsAny<string>()))
            .ReturnsAsync(targetMetadata);
        smCoreMock.Setup(x => x.ContentMetadataService).Returns(smContentServiceMock.Object);

        // Setup the Source Image (SourceMediaData)
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists("A:\\2023 - TestTitle")).Returns(true);   
        var fiMock = new Mock<IFileInfo>();
        var fsiMock = new Mock<IFileSystemInfo>();
        fsiMock.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock.SetupGet(x => x.FullName).Returns("FILENAME.JPG");
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);   
        var sourceMediaDataMock = new Mock<SourceMediaData>(fileSystemMock.Object, fsiMock.Object);

        // Setup the ImageUploaderService
        var imageUploaded = new ImageUploaderService();
        var smImageUploaderSvcMock = new Mock<ImageUploaderService>(smCoreMock.Object);
        smImageUploaderSvcMock.Setup(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<FileMetaContent>())).ReturnsAsync("/api/v2/album/TestKey/image/ImageKey-0");
        smCoreMock.Setup(x => x.ImageUploaderService).Returns(smImageUploaderSvcMock.Object);

        ////////////////////////////////////////////////
        // Perform the test AS NORMAL
        var inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetCreate", "Normal"}
        };
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);

        fiMock.SetupGet(x => x.Length).Returns(1);
        targetMetadata.VideoLength = TimeSpan.FromMinutes(1);
        targetMetadata.IsVideo = false;

        var result = await targetAlbumRepository.ProcessNewRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageUploaderSvcMock.Verify(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<FileMetaContent>()));
        smImageUploaderSvcMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the test AS NONELOG
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetCreate", "NoneLog"}
        };
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());

        fiMock.SetupGet(x => x.Length).Returns(1);
        targetMetadata.VideoLength = TimeSpan.FromMinutes(1);
        targetMetadata.IsVideo = false;

        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessNewRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, uploadThrottler);

        Assert.AreEqual(false, result, "Expected to not process the remote media. ");
        smImageUploaderSvcMock.Verify(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<FileMetaContent>()), Times.Never());
        smImageUploaderSvcMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with IncludeVideos = FALSE
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetCreate", "Normal"},
            {"includeVideos", "false"},
        };

        fiMock.SetupGet(x => x.Length).Returns(1);
        targetMetadata.VideoLength = TimeSpan.FromMinutes(1);
        targetMetadata.IsVideo = true;

        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessNewRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, uploadThrottler);

        Assert.AreEqual(false, result, "Expected to not process the remote media. ");
        smImageUploaderSvcMock.Verify(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<FileMetaContent>()), Times.Never());
        smImageUploaderSvcMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with IncludeVideos = TRUE
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetCreate", "Normal"},
            {"includeVideos", "true"},
        };

        fiMock.SetupGet(x => x.Length).Returns(1);
        targetMetadata.VideoLength = TimeSpan.FromMinutes(1);
        targetMetadata.IsVideo = true;

        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessNewRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageUploaderSvcMock.Verify(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<FileMetaContent>()));
        smImageUploaderSvcMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with a Video over 20 minutes long
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetCreate", "Normal"},
            {"includeVideos", "true"},
        };

        fiMock.SetupGet(x => x.Length).Returns(1);
        targetMetadata.VideoLength = TimeSpan.FromMinutes(25);
        targetMetadata.IsVideo = true;

        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessNewRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, uploadThrottler);

        Assert.AreEqual(false, result, "Expected to not process the remote media. ");
        smImageUploaderSvcMock.Verify(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<FileMetaContent>()), Times.Never());
        smImageUploaderSvcMock.Invocations.Clear();

        ////////////////////////////////////////////////
        // Perform the Image Video AS NORMAL with a Video over 2 Gigs in size
        inMemoryRuntimeSettings = new Dictionary<string, string?> {
            {"targetCreate", "Normal"},
            {"includeVideos", "true"},
        };

        fiMock.SetupGet(x => x.Length).Returns(2000000001);
        targetMetadata.VideoLength = TimeSpan.FromMinutes(1);
        targetMetadata.IsVideo = true;
        targetMetadata.FileInfo = fiMock.Object;

        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        result = await targetAlbumRepository.ProcessNewRemoteMedia(
            runtimeConfig, sourceMediaDataMock.Object, albDetail, uploadThrottler);

        Assert.AreEqual(false, result, "Expected to not process the remote media. ");
        smImageUploaderSvcMock.Verify(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<FileMetaContent>()), Times.Never());
        smImageUploaderSvcMock.Invocations.Clear();
    }
    [TestMethod]
    public async Task ProcessExistingRemoteMedia_Scenario2_UndatedVideo_UploadAgain()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\2023 - TestTitle"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);
        var smCoreMock = new Mock<SmugMugCore.Net.Core20.SmugMugCore>(new object[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
        var uploadThrottler = new SemaphoreSlim(1);
        var calls = new List<string>();

        // Setup the Target Image, a video without a date taken on SmugMug
        var targetImage = new AlbumImageDetail() { AlbumKey = "TestKey", ImageKey = "SomeKey1", FileName = "TestFile.MOV", Caption = "Song Row Row Row your Boat",
            Uris = new AlbumImageUris() { Image = new UriMetadata() { Uri = "/api/v2/image/SomeKey1-0" } } };
        var smImageServiceMock = new Mock<AlbumImageService>(smCoreMock.Object);
        smImageServiceMock.Setup(x => x.DeleteImage(It.IsAny<string>())).Callback(() => calls.Add("Delete")).ReturnsAsync("/api/v2/image/SomeKey1-0");
        smCoreMock.Setup(x => x.AlbumImageService).Returns(smImageServiceMock.Object);
        var albDetail = new AlbumDetail() { Name = "2023 - TestTitle", AlbumKey = "TestKey" };

        // Setup the Source Metadata, the video file has a date taken
        var targetMetadata = new FileMetaContent() { IsVideo = true, DateTaken = new DateTime(2011, 5, 29), VideoLength = TimeSpan.FromMinutes(1) };
        var smContentServiceMock = new Mock<ContentMetadataService>();
        smContentServiceMock.Setup(x => x.DiscoverMetadata(It.IsAny<string>())).ReturnsAsync(targetMetadata);
        smCoreMock.Setup(x => x.ContentMetadataService).Returns(smContentServiceMock.Object);

        // Setup the Source Image (SourceMediaData)
        var fileSystemMock = new Mock<IFileSystem>();
        var fiMock = new Mock<IFileInfo>();
        var fsiMock = new Mock<IFileSystemInfo>();
        fsiMock.SetupGet(x => x.Name).Returns("TestFile.MOV");
        fsiMock.SetupGet(x => x.FullName).Returns("TESTFILE.MOV");
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);
        fiMock.SetupGet(x => x.Length).Returns(1);
        var sourceMediaDataMock = new Mock<SourceMediaData>(fileSystemMock.Object, fsiMock.Object);

        // Setup the ImageUploaderService
        var smImageUploaderSvcMock = new Mock<ImageUploaderService>(smCoreMock.Object);
        smImageUploaderSvcMock.Setup(x => x.UploadAlbumImage(It.IsAny<string>(), It.IsAny<FileMetaContent>()))
            .Callback(() => calls.Add("Upload")).ReturnsAsync("/api/v2/album/TestKey/image/NewKey-0");
        smCoreMock.Setup(x => x.ImageUploaderService).Returns(smImageUploaderSvcMock.Object);

        ////////////////////////////////////////////////
        // Perform the test AS NORMAL: delete the video first, then upload it again, keeping the caption from SmugMug
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> {
            {"targetUpdate", "Normal"}, {"includeVideos", "true"} }).Build());
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        var result = await targetAlbumRepository.ProcessExistingRemoteMedia(runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        CollectionAssert.AreEqual(new[] { "Delete", "Upload" }, calls, "Expected the video to be deleted, then uploaded again");
        smImageServiceMock.Verify(x => x.DeleteImage("/api/v2/image/SomeKey1-0"));
        Assert.AreEqual("Song Row Row Row your Boat", targetMetadata.Caption, "Expected the caption on SmugMug to be kept");
        calls.Clear();

        ////////////////////////////////////////////////
        // Perform the test AS NONELOG: nothing is deleted or uploaded
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> {
            {"targetUpdate", "NoneLog"}, {"includeVideos", "true"} }).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.ProcessExistingRemoteMedia(runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);
        Assert.AreEqual(0, calls.Count, "Expected nothing to be deleted or uploaded");

        ////////////////////////////////////////////////
        // Perform the test with a date taken on SmugMug: nothing is deleted or uploaded
        targetImage.DateTimeOriginal = new DateTimeOffset(2011, 5, 29, 0, 0, 0, TimeSpan.Zero);
        runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> {
            {"targetUpdate", "Normal"}, {"includeVideos", "true"} }).Build());
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.ProcessExistingRemoteMedia(runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);
        Assert.AreEqual(0, calls.Count, "Expected a dated video to not be uploaded again");

        ////////////////////////////////////////////////
        // Perform the test with a dated video with different keywords: only the metadata is updated
        smImageServiceMock.Setup(x => x.UpdateAlbumImage(It.IsAny<AlbumImageDetail>())).Callback(() => calls.Add("UpdateMetadata")).ReturnsAsync(new ImageDetail());
        targetMetadata.Keywords = ["Music", "Accordion"];
        targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        _ = await targetAlbumRepository.ProcessExistingRemoteMedia(runtimeConfig, sourceMediaDataMock.Object, albDetail, targetImage, uploadThrottler);
        CollectionAssert.AreEqual(new[] { "UpdateMetadata" }, calls, "Expected only the metadata of a dated video to be updated");
    }
}
