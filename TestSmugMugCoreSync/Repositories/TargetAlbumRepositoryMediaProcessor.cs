using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
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
public class TargetAlbumRepositoryMediaProcessor
{
    [TestMethod]
    public async Task ProcessExistingRemoteMedia_Redownload_FileLength0()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\2023 - TestTitle"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);
        var smCoreMock = new Mock<SmugMugCore>(new object[]{string.Empty, string.Empty, string.Empty, string.Empty});

        // Setup the Runtime Flags
        var inMemoryRuntimeSettings = new Dictionary<string, string?> {{"sourceRedownload", "Normal"}};
        var runtimeConfig = new RuntimeFlagsConfig(new ConfigurationBuilder().AddInMemoryCollection(inMemoryRuntimeSettings).Build());
        var uploadThrottler = new SemaphoreSlim(runtimeConfig.ImageUploadThrottle);

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
        fiMock.SetupGet(x => x.Length).Returns(0);
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);   
        var sourceMediaData = new SourceMediaData(fileSystemMock.Object, fsiMock.Object);

        // Perform the test
        var targetAlbumRepository = new TargetAlbumRepository(smCoreMock.Object, folderConfig);
        var result = await targetAlbumRepository.ProcessExistingRemoteMedia(
            runtimeConfig, sourceMediaData, albDetail, targetImage, uploadThrottler);

        Assert.AreEqual(true, result, "Expected to process the remote media. ");
        smImageServiceMock.Verify(x => x.DownloadImage(targetImage, It.IsAny<string>()));
    }
}
