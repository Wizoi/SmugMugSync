using Moq;
using System.IO.Abstractions;
using SmugMugCoreSync.Data;

namespace TestSmugMugCoreSync;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void SetupBaseObject()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOODIRECTORY");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(false);   

        var sourceDirectoryData = new SourceDirectoryData(fileSystemMock.Object, directoryInfoMock.Object);

        Assert.IsFalse(sourceDirectoryData.IsLinked, "The object should not be linked.");
        Assert.AreEqual(0, sourceDirectoryData.AlbumId, "Album ID should be 0");
        Assert.AreEqual("", sourceDirectoryData.AlbumKey, "Album Key should be empty");
        Assert.AreEqual("FOODIRECTORY", sourceDirectoryData.FolderName, "Foldernme shoul be the foo directory");
    }
}