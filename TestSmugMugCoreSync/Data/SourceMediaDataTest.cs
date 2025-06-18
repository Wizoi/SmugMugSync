using Moq;
using Moq.Protected;
using SmugMugCore.Net.Service;
using SmugMugCoreSync.Data;
using System.IO.Abstractions;

namespace TestSmugMugCoreSync;

[TestClass]
public class SourceMediaDataTest
{
    [TestMethod]
    public void CoreFieldTest()
    {
        var fiMock = new Mock<IFileInfo>();
        fiMock.SetupGet(x => x.Length).Returns(1000);
        fiMock.SetupGet(x => x.LastWriteTime).Returns(DateTime.Parse("2023-01-01"));        

        var fsiMock = new Mock<IFileSystemInfo>();
        fsiMock.SetupGet(x => x.FullName).Returns("A:\\Foo\\Filename.JPG");
        fsiMock.SetupGet(x => x.Name).Returns("Filename.JPG");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);   

        var sourceMediaData = new SourceMediaData(fileSystemMock.Object, fsiMock.Object);

        Assert.AreEqual(1000, sourceMediaData.FileLength, "File Length should be 1000");
        Assert.AreEqual("FILENAME.JPG", sourceMediaData.FileName, "Filename Expected");
        Assert.AreEqual("A:\\FOO\\FILENAME.JPG", sourceMediaData.FullFileName, "Full Filename Expected");
        Assert.AreEqual("FILENAME", sourceMediaData.FileNameBase, "Filename Base Expected");
        Assert.AreEqual(DateTime.Parse("2023-01-01"), sourceMediaData.LastWriteTime, "Last Write Time Expected");
        Assert.IsTrue(sourceMediaData.IsImageUpdatable(), "Return that  the image, if not a TIF, is updateable to smugmug");
    }

    [TestMethod]
    public void NotUpdatableTest()
    {
        var fiMock = new Mock<IFileInfo>();
        var fsiMock = new Mock<IFileSystemInfo>();
        fsiMock.SetupGet(x => x.Name).Returns("Filename.TIF");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);   

        var sourceMediaData = new SourceMediaData(fileSystemMock.Object, fsiMock.Object);

        Assert.IsFalse(sourceMediaData.IsImageUpdatable(), "TIF Files are not updatable");
    }

    [TestMethod]
    public void LoadFromAdapterTest()
    {
        var fiMock = new Mock<IFileInfo>();
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);   

        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.Name).Returns("Filename1.TIF");

        var fsiMock2 = new Mock<IFileSystemInfo>();
        fsiMock2.SetupGet(x => x.Name).Returns("Filename2.TIF");

        IFileSystemInfo[] fsList = { fsiMock1.Object, fsiMock2.Object};
        SourceMediaData[] mediaDataList = SourceMediaData.LoadFrom(fileSystemMock.Object, fsList);

        Assert.AreEqual(2, mediaDataList.Length, "Media data should have 2 entries.");
        Assert.AreEqual("FILENAME1.TIF", mediaDataList[0].FileName, "First Media File");
        Assert.AreEqual("FILENAME2.TIF", mediaDataList[1].FileName, "Second Media File");
    }
}