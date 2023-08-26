using Moq;
using System.IO.Abstractions;
using SmugMugCoreSync.Data;
using SmugMugCoreSync.Utility;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace TestSmugMugCoreSync;

[TestClass]
public class DirectoryInfoTests
{
    [TestMethod]
    public void DirectoryInfo_NotLinked()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOODIRECTORY");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(false);   

        var xmlSystemMock = new Mock<XmlSystem>();

        var sourceDirectoryData = new SourceDirectoryData(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock.Object);

        Assert.IsFalse(sourceDirectoryData.IsLinked, "The object should not be linked.");
        Assert.AreEqual(0, sourceDirectoryData.AlbumId, "Album ID should be 0");
        Assert.AreEqual("", sourceDirectoryData.AlbumKey, "Album Key should be empty");
        Assert.AreEqual("FOODIRECTORY", sourceDirectoryData.FolderName, "Foldername shoul be the foo directory");
    }

    [TestMethod]
    public void DirectoryInfo_Linked()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOODIRECTORY");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.File.ReadAllText(It.IsAny<string>())).Returns(
            "<smugMugSyncData><albumId>1</albumId><albumKey>someKey</albumKey></smugMugSyncData>");   

        var xmlSystemMock = new Mock<XmlSystem>();

        var sourceDirectoryData = new SourceDirectoryData(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock.Object);

        Assert.IsTrue(sourceDirectoryData.IsLinked, "The object should be linked.");
        Assert.AreEqual(1, sourceDirectoryData.AlbumId, "Album ID should be 0");
        Assert.AreEqual("someKey", sourceDirectoryData.AlbumKey, "Album Key should be empty");
    }

    [TestMethod]
    [ExpectedException(typeof(XmlException))]
    public void DirectoryInfo_NotLinked_BadXml_Exception()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOODIRECTORY");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.File.ReadAllText(It.IsAny<string>())).Returns(
            "<smugMugSyncDataFoo /");   

        var xmlSystemMock = new Mock<XmlSystem>();

        var sourceDirectoryData = new SourceDirectoryData(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock.Object);
    }

    [TestMethod]
    public void DirectoryInfo_NotLinked_BadElements()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOODIRECTORY");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.File.ReadAllText(It.IsAny<string>())).Returns(
            "<smugMugSyncData><albumIdFoo>1</albumIdFoo><albumKeyBar>someKey</albumKeyBar></smugMugSyncData>");   

        var xmlSystemMock = new Mock<XmlSystem>();

        var sourceDirectoryData = new SourceDirectoryData(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock.Object);

        Assert.IsFalse(sourceDirectoryData.IsLinked, "The object should not be linked.");
        Assert.AreEqual(0, sourceDirectoryData.AlbumId, "Album ID should be 0");
        Assert.AreEqual("", sourceDirectoryData.AlbumKey, "Album Key should be empty");
    }

    [TestMethod]
    public void DirectoryInfo_LinkAndSave()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOODIRECTORY");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(false);   
        fileSystemMock.Setup(x => x.File.SetAttributes(It.IsAny<string>(), FileAttributes.Hidden));   

        var xmlSystemMock = new Mock<XmlSystem>();
        xmlSystemMock.Setup(x => x.OutputXmlToFile(It.IsAny<string>(), It.IsAny<XElement>()));

        var sourceDirectoryData = new SourceDirectoryData(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock.Object);
        sourceDirectoryData.LinkToAlbum(1, "fooBarAlbum");

        xmlSystemMock.Verify(x => x.OutputXmlToFile(It.IsAny<string>(), It.IsAny<XElement>()));
        Assert.IsTrue(sourceDirectoryData.IsLinked, "The object should be linked.");
        Assert.AreEqual(1, sourceDirectoryData.AlbumId, "Album ID should be 0");
        Assert.AreEqual("fooBarAlbum", sourceDirectoryData.AlbumKey, "Album Key should be empty");
    }

    [TestMethod]
    public void DirectoryInfo_Link_ThenUnlink()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOODIRECTORY");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.File.ReadAllText(It.IsAny<string>())).Returns(
            "<smugMugSyncData><albumId>1</albumId><albumKey>someKey</albumKey></smugMugSyncData>");   

        var xmlSystemMock = new Mock<XmlSystem>();

        var sourceDirectoryData = new SourceDirectoryData(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock.Object);
        Assert.IsTrue(sourceDirectoryData.IsLinked, "The object should be linked.");
        Assert.AreEqual(1, sourceDirectoryData.AlbumId, "Album ID should be 0");
        Assert.AreEqual("someKey", sourceDirectoryData.AlbumKey, "Album Key should be empty");

        sourceDirectoryData.UnlinkFromAlbum();
        Assert.IsFalse(sourceDirectoryData.IsLinked, "The object should not be linked.");
        Assert.AreEqual(0, sourceDirectoryData.AlbumId, "Album ID should be 0");
        Assert.AreEqual("", sourceDirectoryData.AlbumKey, "Album Key should be empty");
    }    
}