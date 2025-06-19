using Microsoft.Extensions.Configuration;
using Moq;
using SmugMugCoreSync.Configuration;
using SmugMugCoreSync.Data;
using SmugMugCoreSync.Repositories;
using SmugMugCoreSync.Utility;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Xml.Linq;


namespace TestSmugMugCore.SyncApplication.RepositoryTests;

[TestClass]
public class SourceFolderRepositoryTest
{
    [TestMethod]
    public void ConstructorTest()
    {
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "Root Path Value"},
            {"rootRemote", "Remote Path Value"},
            {"filterFolderName", "FILTER NAME"},
            {"folderNamesToSkip:1", "First folder to skip"},
            {"folderNamesToSkip:2", "Second folder to skip"},
            {"extensionsToSkip:1", "Extension to skip"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
    }

    [TestMethod]
    public void PopulateSourceFoldersAndFiles_TopLevelFunction_LocalRoot()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOODIRECTORY\\SKIPME");
        directoryInfoMock.SetupGet(x => x.Name).Returns("SKIPME");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.DirectoryInfo.New(It.IsAny<string>())).Returns(directoryInfoMock.Object);   
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\FOODIRECTORY"},
            {"rootRemote", "A:\\ROOTFOLDER"},
            {"folderNamesToSkip:2", "SKIPME"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var result = sourceFolderRepo.PopulateSourceFoldersAndFiles();

        fileSystemMock.Verify(x => x.DirectoryInfo.New("A:\\FOODIRECTORY"));
        Assert.IsFalse(result, "Loading should not proceed if the folder is on skip list.");
    }

    [TestMethod]
    public void PopulateSourceFoldersAndFiles_TopLevelFunction_RemoteRoot()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOODIRECTORY\\SKIPME");
        directoryInfoMock.SetupGet(x => x.Name).Returns("SKIPME");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.DirectoryInfo.New(It.IsAny<string>())).Returns(directoryInfoMock.Object);   
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   

        // There are trace calls we want to verify are being made
        var mockTrace = new Mock<TraceListener>();
        Trace.Listeners.Add(mockTrace.Object);

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootRemote", "A:\\FOODIRECTORY"},
            {"folderNamesToSkip:2", "SKIPME"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var result = sourceFolderRepo.PopulateSourceFoldersAndFiles();

        mockTrace.Verify(x => x.Write(It.IsAny<string>()), Times.Exactly(1), "Expected a trace");
        mockTrace.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Exactly(2), "Expected a trace");

        fileSystemMock.Verify(x => x.DirectoryInfo.New("A:\\FOODIRECTORY"));
        Assert.IsFalse(result, "Loading should not proceed if the folder is on skip list.");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_SkipSameFolder()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOODIRECTORY\\SKIPME");
        directoryInfoMock.SetupGet(x => x.Name).Returns("SKIPME");

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.DirectoryInfo.New(It.IsAny<string>())).Returns(directoryInfoMock.Object);   
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\FOODIRECTORY"},
            {"folderNamesToSkip:1", "SKIPYOU"},
            {"folderNamesToSkip:2", "SKIPME"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var result = sourceFolderRepo.LoadFoldersAndFiles("A:\\TestFolder");

        Assert.IsFalse(result, "Loading should not proceed if the folder is on skip list.");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_SkipExtensionFilter()
    {
        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.FullName).Returns("A:\\Foo\\Filename.TXT");
        fsiMock1.SetupGet(x => x.Name).Returns("Filename.TXT");
        fsiMock1.SetupGet(x => x.Extension).Returns(".TXT");

        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\Foo");
        directoryInfoMock.SetupGet(x => x.Name).Returns("Foo");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock1.Object});

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.DirectoryInfo.New(It.IsAny<string>())).Returns(directoryInfoMock.Object);           
        fileSystemMock.Setup(x => x.Directory.GetDirectories(It.IsAny<string>())).Returns([]);

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\Foo"},
            {"extensionsToSkip:1", "txt"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var results = sourceFolderRepo.LoadFoldersAndFiles("A:\\TestFolder");

        Assert.IsFalse(results, "No Files should be found to process");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_Unlinked_NoFilters()
    {
        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.FullName).Returns("A:\\Foo\\Filename.JPG");
        fsiMock1.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock1.SetupGet(x => x.Extension).Returns(".JPG");

        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\Foo");
        directoryInfoMock.SetupGet(x => x.Name).Returns("Foo");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock1.Object});

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.File.Exists("A:\\Foo\\.SMUGMUG.INI")).Returns(false);   
        fileSystemMock.Setup(x => x.DirectoryInfo.New(It.IsAny<string>())).Returns(directoryInfoMock.Object);           

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\Foo"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var results = sourceFolderRepo.LoadFoldersAndFiles("A:\\TestFolder");

        Assert.IsTrue(results, "Expected to load a file for the folder.");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveLinkedFolders().Count(), "No linked folders should be loaded");
        Assert.AreEqual(1, sourceFolderRepo.RetrieveUnlinkedFolders().Count(), "1 unlinked folder is expected");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_FolderFilter_NotMatch()
    {
        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.FullName).Returns("A:\\FooBar\\Filename.JPG");
        fsiMock1.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock1.SetupGet(x => x.Extension).Returns(".JPG");

        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FooBar");
        directoryInfoMock.SetupGet(x => x.Name).Returns("FooBar");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock1.Object});

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.DirectoryInfo.New(It.IsAny<string>())).Returns(directoryInfoMock.Object);           

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\FooBar"},
            {"filterFolderName", "Ignore"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var results = sourceFolderRepo.LoadFoldersAndFiles("A:\\FooBar");

        Assert.IsFalse(results, "Folder should be filtered out, and nothing loaded.");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveLinkedFolders().Count(), "No linked folders should be loaded");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveUnlinkedFolders().Count(), "No unlinked folders should be loaded");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_FolderFilter_Match_Caps()
    {
        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.FullName).Returns("A:\\FooBar\\Filename.JPG");
        fsiMock1.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock1.SetupGet(x => x.Extension).Returns(".JPG");

        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FooBar");
        directoryInfoMock.SetupGet(x => x.Name).Returns("FooBar");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock1.Object});

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.File.Exists("A:\\FooBar\\.SMUGMUG.INI")).Returns(false);   
        fileSystemMock.Setup(x => x.DirectoryInfo.New(It.IsAny<string>())).Returns(directoryInfoMock.Object);           

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\FooBar"},
            {"filterFolderName", "BAR"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var results = sourceFolderRepo.LoadFoldersAndFiles("A:\\FooBar");

        Assert.IsTrue(results, "Expected to load a file for the folder.");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveLinkedFolders().Count(), "No linked folders should be loaded");
        Assert.AreEqual(1, sourceFolderRepo.RetrieveUnlinkedFolders().Count(), "1 unlinked folder is expected");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_FolderFilter_Match_MixedCase()
    {
        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.FullName).Returns("A:\\FooBar\\Filename.JPG");
        fsiMock1.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock1.SetupGet(x => x.Extension).Returns(".JPG");

        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FooBar");
        directoryInfoMock.SetupGet(x => x.Name).Returns("FooBar");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock1.Object});

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.File.Exists("A:\\FooBar\\.SMUGMUG.INI")).Returns(false);   
        fileSystemMock.Setup(x => x.DirectoryInfo.New(It.IsAny<string>())).Returns(directoryInfoMock.Object);           

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\FooBar"},
            {"filterFolderName", "Bar"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var results = sourceFolderRepo.LoadFoldersAndFiles("A:\\FooBar");

        Assert.IsTrue(results, "Expected to load a file for the folder.");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveLinkedFolders().Count(), "No linked folders should be loaded");
        Assert.AreEqual(1, sourceFolderRepo.RetrieveUnlinkedFolders().Count(), "1 unlinked folder is expected");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_Linked()
    {
        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.FullName).Returns("A:\\Foo\\Filename.JPG");
        fsiMock1.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock1.SetupGet(x => x.Extension).Returns(".JPG");

        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\Foo");
        directoryInfoMock.SetupGet(x => x.Name).Returns("Foo");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock1.Object});

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.DirectoryInfo.New(It.IsAny<string>())).Returns(directoryInfoMock.Object);           
        fileSystemMock.Setup(x => x.File.Exists("A:\\Foo\\.SMUGMUG.INI")).Returns(true);   
        fileSystemMock.Setup(x => x.File.ReadAllText("A:\\Foo\\.SMUGMUG.INI")).Returns(
            "<smugMugSyncData><albumId>1</albumId><albumKey>someKey</albumKey></smugMugSyncData>");   

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\Foo"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var results = sourceFolderRepo.LoadFoldersAndFiles("A:\\FooBar");

        Assert.IsTrue(results, "Expected to load a file for the folder.");
        Assert.AreEqual(1, sourceFolderRepo.RetrieveLinkedFolders().Count(), "1 linked folder should be loaded");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveUnlinkedFolders().Count(), "0 unlinked folders are expected");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_NoFilesOrFolders()
    {
        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\Foo");
        directoryInfoMock.SetupGet(x => x.Name).Returns("Foo");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns([]);

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.DirectoryInfo.New(It.IsAny<string>())).Returns(directoryInfoMock.Object);           
        fileSystemMock.Setup(x => x.Directory.GetDirectories(It.IsAny<string>())).Returns([]);

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\Foo"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var results = sourceFolderRepo.LoadFoldersAndFiles("A:\\Foo");

        Assert.IsFalse(results, "Nothing was found.");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_Subfolder_Match_And_Add()
    {
        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.FullName).Returns("A:\\Foo\\Bar\\Filename.JPG");
        fsiMock1.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock1.SetupGet(x => x.Extension).Returns(".JPG");

        var subDirectoryInfoMock = new Mock<IDirectoryInfo>();
        subDirectoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\Foo\\Bar");
        subDirectoryInfoMock.SetupGet(x => x.Name).Returns("Bar");
        subDirectoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock1.Object});

        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\Foo");
        directoryInfoMock.SetupGet(x => x.Name).Returns("Foo");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns([]);

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.File.Exists("A:\\Foo\\Bar\\.SMUGMUG.INI")).Returns(false);   
        fileSystemMock.Setup(x => x.DirectoryInfo.New("A:\\Foo")).Returns(directoryInfoMock.Object);           
        fileSystemMock.Setup(x => x.DirectoryInfo.New("A:\\Foo\\Bar")).Returns(subDirectoryInfoMock.Object);           
        fileSystemMock.Setup(x => x.Directory.GetDirectories("A:\\Foo")).Returns(["A:\\Foo\\Bar"]);

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\Foo"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var results = sourceFolderRepo.LoadFoldersAndFiles("A:\\Foo");

        // Check that we recursively went over both folders
        fileSystemMock.Verify(x => x.DirectoryInfo.New("A:\\Foo"));
        fileSystemMock.Verify(x => x.DirectoryInfo.New("A:\\Foo\\Bar"));
        // Check the results
        Assert.IsTrue(results, "Expected to load a file for the folder.");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveLinkedFolders().Count(), "No linked folders should be loaded");
        var folders = sourceFolderRepo.RetrieveUnlinkedFolders();
        Assert.AreEqual(1, folders.Count(), "1 unlinked folder is expected");
        Assert.AreEqual("Bar", folders.First().FolderName, "The Bar folder should be  added.");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_Subfolder_Multiple_Match_And_Add()
    {
        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.FullName).Returns("A:\\Foo\\Bar\\Filename.JPG");
        fsiMock1.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock1.SetupGet(x => x.Extension).Returns(".JPG");

        var fsiMock2 = new Mock<IFileSystemInfo>();
        fsiMock2.SetupGet(x => x.FullName).Returns("A:\\Foo\\Cat\\Filename.JPG");
        fsiMock2.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock2.SetupGet(x => x.Extension).Returns(".JPG");

        var subDirectoryInfoMock1 = new Mock<IDirectoryInfo>();
        subDirectoryInfoMock1.SetupGet(x => x.FullName).Returns("A:\\Foo\\Bar");
        subDirectoryInfoMock1.SetupGet(x => x.Name).Returns("Bar");
        subDirectoryInfoMock1.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock1.Object});

        var subDirectoryInfoMock2 = new Mock<IDirectoryInfo>();
        subDirectoryInfoMock2.SetupGet(x => x.FullName).Returns("A:\\Foo\\Cat");
        subDirectoryInfoMock2.SetupGet(x => x.Name).Returns("Cat");
        subDirectoryInfoMock2.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock2.Object});

        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\Foo");
        directoryInfoMock.SetupGet(x => x.Name).Returns("Foo");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns([]);

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.File.Exists("A:\\Foo\\Bar\\.SMUGMUG.INI")).Returns(false);   
        fileSystemMock.Setup(x => x.File.Exists("A:\\Foo\\Cat\\.SMUGMUG.INI")).Returns(false);   
        fileSystemMock.Setup(x => x.DirectoryInfo.New("A:\\Foo")).Returns(directoryInfoMock.Object);           
        fileSystemMock.Setup(x => x.DirectoryInfo.New("A:\\Foo\\Bar")).Returns(subDirectoryInfoMock1.Object);           
        fileSystemMock.Setup(x => x.DirectoryInfo.New("A:\\Foo\\Cat")).Returns(subDirectoryInfoMock2.Object);           
        fileSystemMock.Setup(x => x.Directory.GetDirectories("A:\\Foo")).Returns(["A:\\Foo\\Bar", "A:\\Foo\\Cat"]);

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\Foo"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var results = sourceFolderRepo.LoadFoldersAndFiles("A:\\Foo");

        // Check that we recursively went over both folders
        fileSystemMock.Verify(x => x.DirectoryInfo.New("A:\\Foo"));
        fileSystemMock.Verify(x => x.DirectoryInfo.New("A:\\Foo\\Bar"));
        fileSystemMock.Verify(x => x.DirectoryInfo.New("A:\\Foo\\Cat"));
        // Check the results
        Assert.IsTrue(results, "Expected to load a file for the folder.");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveLinkedFolders().Count(), "No linked folders should be loaded");
        var folders = sourceFolderRepo.RetrieveUnlinkedFolders();
        Assert.AreEqual(2, folders.Count(), "2 unlinked folder is expected");
        Assert.AreEqual("Bar", folders.First().FolderName, "The Bar folder should be  added.");
        Assert.AreEqual("Cat", folders.Last().FolderName, "The Cat folder should be  added.");
    }

    [TestMethod]
    public void LoadFolderAndFilesTest_Subfolder_Ignored_WithFiles()
    {
        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.FullName).Returns("A:\\Foo\\Bar\\Filename.JPG");
        fsiMock1.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock1.SetupGet(x => x.Extension).Returns(".JPG");

        var subDirectoryInfoMock = new Mock<IDirectoryInfo>();
        subDirectoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\Foo\\Bar");
        subDirectoryInfoMock.SetupGet(x => x.Name).Returns("Bar");
        subDirectoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock1.Object});

        var fsiMock2 = new Mock<IFileSystemInfo>();
        fsiMock2.SetupGet(x => x.FullName).Returns("A:\\Foo\\FilenameA.JPG");
        fsiMock2.SetupGet(x => x.Name).Returns("FilenameA.JPG");
        fsiMock2.SetupGet(x => x.Extension).Returns(".JPG");

        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\Foo");
        directoryInfoMock.SetupGet(x => x.Name).Returns("Foo");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock2.Object});

        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   
        fileSystemMock.Setup(x => x.File.Exists("A:\\Foo\\Bar\\.SMUGMUG.INI")).Returns(false);   
        fileSystemMock.Setup(x => x.DirectoryInfo.New("A:\\Foo\\Bar")).Returns(subDirectoryInfoMock.Object);           
        fileSystemMock.Setup(x => x.DirectoryInfo.New("A:\\Foo")).Returns(directoryInfoMock.Object);           

        var xmlSystemMock = new Mock<XmlSystem>();

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\Foo"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);
        var results = sourceFolderRepo.LoadFoldersAndFiles("A:\\Foo");

        Assert.IsTrue(results, "Expected to load a file for the folder.");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveLinkedFolders().Count(), "No linked folders should be loaded");
        var folders = sourceFolderRepo.RetrieveUnlinkedFolders();
        Assert.AreEqual(1, folders.Count(), "1 unlinked folder is expected");
        Assert.AreEqual("Foo", folders.First().FolderName, "The Bar folder should be  added.");
    }

     [TestMethod]
    public void AddNewLinkedFolder_Tests()
    {
        var xmlSystemMock = new Mock<XmlSystem>();
        xmlSystemMock.Setup(x => x.OutputXmlToFile(It.IsAny<string>(), It.IsAny<XElement>()));

        var fileSystemMock = new Mock<IFileSystem>();

        // Create the two directories we want to add
        var directoryInfoMock1 = new Mock<IDirectoryInfo>();
        directoryInfoMock1.SetupGet(x => x.FullName).Returns("A:\\DIRECTORY1");
        fileSystemMock.Setup(x => x.File.Exists("A:\\DIRECTORY1\\.SMUGMUG.INI")).Returns(true);   
        fileSystemMock.Setup(x => x.File.ReadAllText("A:\\DIRECTORY1\\.SMUGMUG.INI")).Returns(
            "<smugMugSyncData><albumKey>someKey</albumKey></smugMugSyncData>");   

        var sourceDirData1 = new SourceDirectoryData(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock1.Object);

        var directoryInfoMock2 = new Mock<IDirectoryInfo>();
        directoryInfoMock2.SetupGet(x => x.FullName).Returns("A:\\DIRECTORY2");
        fileSystemMock.Setup(x => x.File.Exists("A:\\DIRECTORY2\\.SMUGMUG.INI")).Returns(true);   
        fileSystemMock.Setup(x => x.File.ReadAllText("A:\\DIRECTORY2\\.SMUGMUG.INI")).Returns(
            "<smugMugSyncData><albumKey>someOtherKey</albumKey></smugMugSyncData>");   

        var sourceDirData2 = new SourceDirectoryData(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock2.Object);

        // Setup the base repository
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   

        var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\Foo"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);

        // TESTS:  Add folders, even duplicates and verify they add as expected
        Assert.AreEqual(0, sourceFolderRepo.RetrieveLinkedFolders().Count(), "0 linked folders are expected");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveUnlinkedFolders().Count(), "0 unlinked folders are expected");
       
        sourceFolderRepo.AddNewLinkedFolder(sourceDirData2);
        sourceFolderRepo.AddNewLinkedFolder(sourceDirData1);
        sourceFolderRepo.AddNewLinkedFolder(sourceDirData1);
        sourceFolderRepo.AddNewLinkedFolder(sourceDirData2);

        Assert.AreEqual(2, sourceFolderRepo.RetrieveLinkedFolders().Count(), "2 linked folders are expected");
        Assert.AreEqual(0, sourceFolderRepo.RetrieveUnlinkedFolders().Count(), "0 unlinked folders are expected");

        // Verify when they are linked, we can retrieve them
        var actualFolder = sourceFolderRepo.RetrieveLinkedFolderByKey("someKey");
        Assert.IsNotNull(actualFolder);
        Assert.AreEqual("someKey", actualFolder.AlbumKey, "Album should be found and match the key");

        var noActualFolder = sourceFolderRepo.RetrieveLinkedFolderByKey("someKeyMiss");
        Assert.IsNull(noActualFolder);


        // TESTS:  Remove them and verify they become unlinked
        sourceFolderRepo.RemoveLinkedFolder(sourceDirData1);
        sourceFolderRepo.RemoveLinkedFolder(sourceDirData2);

        Assert.AreEqual(0, sourceFolderRepo.RetrieveLinkedFolders().Count(), "0 linked folders are expected");
        Assert.AreEqual(2, sourceFolderRepo.RetrieveUnlinkedFolders().Count(), "0 unlinked folders are expected");
    }

    [TestMethod]
    public void LoadFolderMediaFiles()
    {
        
        // Create the Folder Repository
        var fileSystemMock = new Mock<IFileSystem>();
        fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);   

        var xmlSystemMock = new Mock<XmlSystem>();

         var inMemorySettings = new Dictionary<string, string?> {
            {"rootLocal", "A:\\Foo"},
            {"extensionsToSkip:1", "txt"},
        };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var folderConfig = new FolderSyncPathsConfig(configuration);

        var sourceFolderRepo = new SourceFolderRepository(fileSystemMock.Object, xmlSystemMock.Object, folderConfig);

        // Create the SourceDirectoryData objects to pass in

        var fsiMock1 = new Mock<IFileSystemInfo>();
        fsiMock1.SetupGet(x => x.FullName).Returns("A:\\Foo\\Filename.TXT");
        fsiMock1.SetupGet(x => x.Name).Returns("Filename.TXT");
        fsiMock1.SetupGet(x => x.Extension).Returns(".TXT");

        var fsiMock2 = new Mock<IFileSystemInfo>();
        fsiMock2.SetupGet(x => x.FullName).Returns("A:\\Foo\\Filename.JPG");
        fsiMock2.SetupGet(x => x.Name).Returns("Filename.JPG");
        fsiMock2.SetupGet(x => x.Extension).Returns(".JPG");

        var directoryInfoMock = new Mock<IDirectoryInfo>();
        directoryInfoMock.SetupGet(x => x.FullName).Returns("A:\\FOO");
        directoryInfoMock.Setup(x => x.GetFileSystemInfos()).Returns(new []{fsiMock1.Object, fsiMock2.Object});

        // For the mock to the SourceMediaData object
        var fiMock = new Mock<IFileInfo>();

        fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(false);   
        fileSystemMock.Setup(x => x.FileInfo.New(It.IsAny<string>())).Returns(fiMock.Object);           

        // Create two objects
        var sourceDirData = new SourceDirectoryData(fileSystemMock.Object, xmlSystemMock.Object, directoryInfoMock.Object);

        // After the setup, run the test
        var outputData = sourceFolderRepo.LoadFolderMediaFiles(sourceDirData);

        Assert.AreEqual("FILENAME.JPG", outputData[0].FileName, "JPG Filename");
        Assert.AreEqual(1, outputData.Length, "1 Directory should be returned, other filtered due to being a TXT file.");
    }
}