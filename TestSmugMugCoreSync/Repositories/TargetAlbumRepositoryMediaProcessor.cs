using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using SmugMugCore.Net.Core;
using SmugMugCore.Net.Data;
using SmugMugCore.Net.Service;
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

namespace TestSmugMugCoreSync;

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
        var smCoreMock = new Mock<SmugMugCore.Net.Core.SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});
        var uploadThrottler = new SemaphoreSlim(1);

        // Setup the Target Image (ImageDetail)
        var targetImage = new ImageDetail() { ImageKey = "SomeKey1", Filename = "TestFile.JPG" };
        var smImageServiceMock = new Mock<ImageService>(smCoreMock.Object);
        smImageServiceMock.Setup(x => x.GetImageInfo(
            It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(targetImage);
        smImageServiceMock.Setup(x => x.DownloadImage(targetImage, It.IsAny<string>())).ReturnsAsync(true);            
        smCoreMock.Setup(x => x.ImageService).Returns(smImageServiceMock.Object);

        // Setup the Target Album (AlbumDetail) / ALBUMSERVICE
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        albDetail.Images = new []{targetImage};
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        // Setup the Source Metadata (ImageContent) / CONTENTMETADATASERVICE
        var targetMetadata = new ImageContent();
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
        smImageServiceMock.Verify(x => x.DownloadImage(targetImage, It.IsAny<string>()));
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
        smImageServiceMock.Verify(x => x.DownloadImage(targetImage, It.IsAny<string>()), Times.Never());
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
        smImageServiceMock.Verify(x => x.DownloadImage(targetImage, It.IsAny<string>()), Times.Never());
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
        smImageServiceMock.Verify(x => x.DownloadImage(targetImage, It.IsAny<string>()));
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
        var smCoreMock = new Mock<SmugMugCore.Net.Core.SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});
        var uploadThrottler = new SemaphoreSlim(1);

        // Setup the Target Image (ImageDetail)
        var targetImage = new ImageDetail() { ImageKey = "SomeKey1", Filename = "TestFile.JPG" };
        var smImageServiceMock = new Mock<ImageService>(smCoreMock.Object);
        smImageServiceMock.Setup(x => x.GetImageInfo(
            It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(targetImage);
        smImageServiceMock.Setup(x => x.DownloadImage(targetImage, It.IsAny<string>())).ReturnsAsync(true);            
        smCoreMock.Setup(x => x.ImageService).Returns(smImageServiceMock.Object);

        // Setup the Target Album (AlbumDetail) / ALBUMSERVICE
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        albDetail.Images = new []{targetImage};
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        // Setup the Source Metadata (ImageContent) / CONTENTMETADATASERVICE
        var targetMetadata = new ImageContent();
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
        var imageUploaded = new ImageUpload();
        var smImageUploaderSvcMock = new Mock<ImageUploaderService>(smCoreMock.Object);
        smImageUploaderSvcMock.Setup(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>())).ReturnsAsync(imageUploaded);
        smCoreMock.Setup(x => x.ImageUploaderService).Returns(smImageUploaderSvcMock.Object);

        // Scenario Setup - Setup the length and size fields to be different, but the source is not 0
        fiMock.SetupGet(x => x.Length).Returns(1);
        targetImage.SizeBytes = 2;
        // Scenario Setup - Setup the checksum to be  different
        sourceMediaDataMock.Setup(x => x.LoadMd5Checksum()).ReturnsAsync("ChecksumData");
        targetImage.MD5Sum = "ChecksumOrigData";
        // Scenario Setup - Mark the date on the file and the local file to be the same
        fiMock.SetupGet(x => x.LastWriteTime).Returns(DateTime.Parse("2020-01-01"));
        targetImage.LastUpdatedDate = DateTime.Parse("2020-01-01").ToString();

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
        smImageUploaderSvcMock.Verify(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>()));
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
        smImageUploaderSvcMock.Verify(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>()), Times.Never());
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
        smImageUploaderSvcMock.Verify(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>()), Times.Never());
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
        smImageUploaderSvcMock.Verify(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>()), Times.Never());
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
        var smCoreMock = new Mock<SmugMugCore.Net.Core.SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});
        var uploadThrottler = new SemaphoreSlim(1);

        // Setup the Target Image (ImageDetail)
        var targetImage = new ImageDetail() { ImageKey = "SomeKey1", Filename = "TestFile.JPG" };
        var smImageServiceMock = new Mock<ImageService>(smCoreMock.Object);
        smImageServiceMock.Setup(x => x.GetImageInfo(
            It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(targetImage);
        smImageServiceMock.Setup(x => x.DownloadImage(targetImage, It.IsAny<string>())).ReturnsAsync(true);            
        smCoreMock.Setup(x => x.ImageService).Returns(smImageServiceMock.Object);

        // Setup the Target Album (AlbumDetail) / ALBUMSERVICE
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        albDetail.Images = new []{targetImage};
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        // Setup the Source Metadata (ImageContent) / CONTENTMETADATASERVICE
        var targetMetadata = new ImageContent();
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
        var imageUploaded = new ImageUpload();
        var smImageUploaderSvcMock = new Mock<ImageUploaderService>(smCoreMock.Object);
        smImageUploaderSvcMock.Setup(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>())).ReturnsAsync(imageUploaded);
        smCoreMock.Setup(x => x.ImageUploaderService).Returns(smImageUploaderSvcMock.Object);

        // Scenario Setup - Setup the length and size fields to be different, but the source is not 0
        fiMock.SetupGet(x => x.Length).Returns(1);
        targetImage.SizeBytes = 2;
        // Scenario Setup - Setup the checksum to be  different
        sourceMediaDataMock.Setup(x => x.LoadMd5Checksum()).ReturnsAsync("ChecksumData");
        targetImage.MD5Sum = "ChecksumOrigData";
        // Scenario Setup - Source file is image dated before the target date (implies it was reprocessed by smugmug)
        targetImage.LastUpdatedDate = DateTime.Parse("2020-01-02").ToString();
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
        smImageServiceMock.Verify(x => x.DownloadImage(targetImage, It.IsAny<string>()));
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
        smImageServiceMock.Verify(x => x.DownloadImage(targetImage, It.IsAny<string>()), Times.Never());
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
        smImageServiceMock.Verify(x => x.DownloadImage(targetImage, It.IsAny<string>()), Times.Never());
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
        smImageServiceMock.Verify(x => x.DownloadImage(targetImage, It.IsAny<string>()), Times.Never());
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
        var smCoreMock = new Mock<SmugMugCore.Net.Core.SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});
        var uploadThrottler = new SemaphoreSlim(1);

        // Setup the Target Image (ImageDetail)
        var targetImage = new ImageDetail() { ImageKey = "SomeKey1", Filename = "TestFile.JPG" };
        var smImageServiceMock = new Mock<ImageService>(smCoreMock.Object);
        smImageServiceMock.Setup(x => x.GetImageInfo(
            It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(targetImage);
        smImageServiceMock.Setup(x => x.UpdateImage(targetImage)).ReturnsAsync(true);
        smImageServiceMock.Setup(x => x.DownloadImage(targetImage, It.IsAny<string>())).ReturnsAsync(true);            
        smCoreMock.Setup(x => x.ImageService).Returns(smImageServiceMock.Object);

        // Setup the Target Album (AlbumDetail) / ALBUMSERVICE
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        albDetail.Images = new []{targetImage};
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        // Setup the Source Metadata (ImageContent) / CONTENTMETADATASERVICE
        var targetMetadata = new ImageContent();
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
        var imageUploaded = new ImageUpload();
        var smImageUploaderSvcMock = new Mock<ImageUploaderService>(smCoreMock.Object);
        smImageUploaderSvcMock.Setup(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>())).ReturnsAsync(imageUploaded);
        smCoreMock.Setup(x => x.ImageUploaderService).Returns(smImageUploaderSvcMock.Object);

        // Scenario Setup - Size, Dates and checksum match
        fiMock.SetupGet(x => x.Length).Returns(1);
        targetImage.SizeBytes = 1;
        sourceMediaDataMock.Setup(x => x.LoadMd5Checksum()).ReturnsAsync("ChecksumData");
        targetImage.MD5Sum = "ChecksumData";
        targetImage.LastUpdatedDate = DateTime.Parse("2020-01-01").ToString();
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
        smImageServiceMock.Verify(x => x.UpdateImage(targetImage));
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
        smImageServiceMock.Verify(x => x.UpdateImage(targetImage), Times.Never());
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
        smImageServiceMock.Verify(x => x.UpdateImage(targetImage), Times.Never());
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
        smImageServiceMock.Verify(x => x.UpdateImage(targetImage));
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
        var smCoreMock = new Mock<SmugMugCore.Net.Core.SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});
        var uploadThrottler = new SemaphoreSlim(1);

        // Setup the Target Album (AlbumDetail) / ALBUMSERVICE
        var smAlbumServiceMock = new Mock<AlbumService>(smCoreMock.Object);
        var albDetail = new AlbumDetail();
        albDetail.Title = "2023 - TestTitle";
        albDetail.AlbumKey = "TestKey";
        smAlbumServiceMock.Setup(x => x.GetAlbumList(It.IsAny<string[]>()))
            .ReturnsAsync(new []{albDetail});
        smCoreMock.Setup(x => x.AlbumService).Returns(smAlbumServiceMock.Object);

        // Setup the Source Metadata (ImageContent) / CONTENTMETADATASERVICE
        var targetMetadata = new ImageContent();
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
        var imageUploaded = new ImageUpload();
        var smImageUploaderSvcMock = new Mock<ImageUploaderService>(smCoreMock.Object);
        smImageUploaderSvcMock.Setup(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>())).ReturnsAsync(imageUploaded);
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
        smImageUploaderSvcMock.Verify(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>()));
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
        smImageUploaderSvcMock.Verify(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>()), Times.Never());
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
        smImageUploaderSvcMock.Verify(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>()), Times.Never());
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
        smImageUploaderSvcMock.Verify(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>()));
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
        smImageUploaderSvcMock.Verify(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>()), Times.Never());
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
        smImageUploaderSvcMock.Verify(x => x.UploadUpdatedImage(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<ImageContent>()), Times.Never());
        smImageUploaderSvcMock.Invocations.Clear();


    }
}

