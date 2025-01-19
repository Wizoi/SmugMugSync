namespace TestSmugMugCoreNetAPI;

/// <summary>
///This is a test class for ContentMetadataLoaderTest and is intended
///to contain all ContentMetadataLoaderTest Unit Tests
///</summary>
[TestClass()]
public class ContentMetadataServiceTest
{
    private TestContext? testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext? TestContext
    {
        get
        {
            return testContextInstance;
        }
        set
        {
            testContextInstance = value;
        }
    }

    /// <summary>
    /// Load the deployment directory from the test context
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ApplicationException">Exception when it fails to load context</exception>
    private string GetDeploymentDirectory()
    {
        if (this.TestContext != null)
        {
            return this.TestContext.TestDeploymentDir;
        }
        else
            throw new ApplicationException("Test Context not populate during test execution.");
    }

    /// <summary>
    ///A test for DiscoverMetadata
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestImage.jpg")]
    public async Task DiscoverMetadataImageJpgTest()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestImage.jpg");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        Assert.AreEqual("Pepper", actual.Title);
        Assert.AreEqual("This is a multi-line comment.", actual.Caption);
        Assert.AreEqual(DateTime.Parse("2012-01-15 06:03:38 PM"), actual.DateTaken);
        Assert.AreEqual(false, actual.IsHidden);
        Assert.AreEqual(false, actual.IsVideo);
        Assert.AreEqual(4, actual.Keywords.Length);
        Assert.AreEqual("Pets", actual.Keywords[0]);
        Assert.AreEqual("Pepper", actual.Keywords[1]);
        Assert.AreEqual("Activity", actual.Keywords[2]);
        Assert.AreEqual("Snow", actual.Keywords[3]);
        Assert.AreEqual(1, (int)actual.Orientation);
        Assert.AreEqual(new TimeSpan(), actual.VideoLength);
    }

    /// <summary>
    ///A test for DiscoverMetadata
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestJpegImage.jpeg")]
    public async Task DiscoverMetadataImageJpegTest()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestJpegImage.jpeg");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        Assert.AreEqual("Test Jpeg Photo", actual.Title);
    }

    /// <summary>
    ///A test for DiscoverMetadata
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestScannedImage.jpg")]
    public async Task DiscoverMetadataImageScannedTest()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestScannedImage.jpg");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        Assert.AreEqual("Test Scanned Photo", actual.Title);
    }

    /// <summary>
    ///A test for DiscoverMetadata
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestBmpImage.bmp")]
    public async Task DiscoverMetadataImageBmpTest()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestBmpImage.bmp");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        Assert.IsNull(actual.Caption, "Caption is not saved, and thus should be null");
    }

    /// <summary>
    ///A test for DiscoverMetadata
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestPngImage.png")]
    public async Task DiscoverMetadataImagePngTest()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestPngImage.png");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        Assert.IsNull(actual.Caption, "Caption is not saved, and thus should be null");
    }

    /// <summary>
    ///A test for DiscoverMetadata
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestTifImage.tif")]
    public async Task DiscoverMetadataImageTifTest()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestTifImage.tif");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        Assert.AreEqual("Test Tif File", actual.Title);
    }

    /// <summary>
    ///A test for DiscoverMetadata
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideo.mov")]
    public async Task DiscoverMetadataVideoTest()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestVideo.mov");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        Assert.AreEqual("This is a test Video", actual.Title, "Caption");
        Assert.AreEqual(false, actual.IsHidden, "IsHidden");
        Assert.AreEqual(true, actual.IsVideo, "IsVideo Flag");
        Assert.AreEqual(4, actual.Keywords.Length, "Count of Keywords");
        Assert.AreEqual("Activity", actual.Keywords.Order().First(), "Keywords");
        Assert.AreEqual(new TimeSpan(95094999), actual.VideoLength, "Video Length is not 1 second");
    }

    /// <summary>
    ///A test for DiscoverMetadata
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideoMp4.mp4")]
    public async Task DiscoverMetadataVideoMp4Test()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestVideoMp4.mp4");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        Assert.AreNotEqual(new TimeSpan(0, 0, 0), actual.VideoLength, "Video Length is returning as 0");
        Assert.AreEqual(true, actual.IsVideo, "IsVideo Flag");
        Assert.AreEqual("Test Mp4 Video", actual.Title, "Caption");
    }

    /// <summary>
    ///A test for DiscoverMetadata
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideoMpg.mpg")]
    public async Task DiscoverMetadataVideoMpgTest()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestVideoMpg.mpg");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        Assert.AreNotEqual(new TimeSpan(0, 0, 0), actual.VideoLength, "Video Length is returning as 0");
        Assert.AreEqual(true, actual.IsVideo, "IsVideo Flag");
        Assert.IsNull(actual.Caption, "Caption will not save, and will be null");
    }

    /// <summary>
    ///A test for DiscoverMetadata
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideoWmv.wmv")]
    public async Task DiscoverMetadataVideoWmvTest()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestVideoWmv.Wmv");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        Assert.AreNotEqual(new TimeSpan(0, 0, 0), actual.VideoLength, "Video Length is returning as 0");
        Assert.AreEqual(true, actual.IsVideo, "IsVideo Flag");
        Assert.AreEqual("Test Wmv Video", actual.Title, "Caption");
    }

    /// <summary>
    ///A test for Comparing Keywords
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideoWmv.wmv")]
    public async Task CompareKeywords_MatchingKeywords()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestVideoWmv.Wmv");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);
        bool result = false;

        actual.Keywords = new string[]{"this", "that"};
        result = core.ContentMetadataService.AreKeywordsDifferent(actual, "this, that");
        Assert.IsFalse(result, "Comparing this and that");

        actual.Keywords = new string[]{"this", "that"};
        result = core.ContentMetadataService.AreKeywordsDifferent(actual, "this; that");
        Assert.IsFalse(result, "Comparing this and that");

        actual.Keywords = [];
        result = core.ContentMetadataService.AreKeywordsDifferent(actual, "");
        Assert.IsFalse(result, "Comparing an empty array");

        actual.Keywords = new string[]{""};
        result = core.ContentMetadataService.AreKeywordsDifferent(actual, "");
        Assert.IsFalse(result, "Comparing an array with one string element");

        actual.Keywords = new string[]{"  "};
        result = core.ContentMetadataService.AreKeywordsDifferent(actual, "  ");
        Assert.IsFalse(result, "Comparing an array with one string element");
    }

    /// <summary>
    ///A test for Comparing Keywords
    ///</summary>
    [TestMethod()]
    [DeploymentItem(@"Content\TestVideoWmv.wmv")]
    public async Task CompareKeywords_NotMatchingKeywords()
    {
        var core = Utility.RetrieveSmugMugCore();;
        string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestVideoWmv.Wmv");
        var actual = await core.ContentMetadataService.DiscoverMetadata(filepath);

        bool result = false;

        actual.Keywords = new string[]{"this", "That"};
        result = core.ContentMetadataService.AreKeywordsDifferent(actual, "This, That");
        Assert.IsTrue(result, "Comparing this and that capitalized");

        actual.Keywords = new string[]{"this", "That"};
        result = core.ContentMetadataService.AreKeywordsDifferent(actual, " this ,  that ");
        Assert.IsTrue(result, "Comparing this and that (spaces)");

        actual.Keywords = new string[]{"this", "That"};
        result = core.ContentMetadataService.AreKeywordsDifferent(actual, "this;  that");
        Assert.IsTrue(result, "Comparing this and that (semi-colon)");

        actual.Keywords = new string[]{"this, that"};
        result = core.ContentMetadataService.AreKeywordsDifferent(actual, "this;  that");
        Assert.IsTrue(result, "Comparing this and that (semi-colon)");
    }
}
