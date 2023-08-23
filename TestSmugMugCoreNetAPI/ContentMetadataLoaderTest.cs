namespace TestSmugMugCoreNetAPI
{
    /// <summary>
    ///This is a test class for ContentMetadataLoaderTest and is intended
    ///to contain all ContentMetadataLoaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ContentMetadataLoaderTest
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
        /// Load the deploiyment directory from the test context
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
        public void DiscoverMetadataImageJpgTest()
        {
            string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestImage.jpg");
            var actual = ContentMetadataLoader.DiscoverMetadata(filepath);
            Assert.AreEqual("Pepper", actual.Caption);
            Assert.AreEqual("This is a multi-line comment.", actual.Comment);
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
        public void DiscoverMetadataImageJpegTest()
        {
            string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestJpegImage.jpeg");
            var actual = ContentMetadataLoader.DiscoverMetadata(filepath);
            Assert.AreEqual("Test Jpeg Photo", actual.Caption);
        }

        /// <summary>
        ///A test for DiscoverMetadata
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Content\TestScannedImage.jpg")]
        public void DiscoverMetadataImageScannedTest()
        {
            string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestScannedImage.jpg");
            var actual = ContentMetadataLoader.DiscoverMetadata(filepath);
            Assert.AreEqual("Test Scanned Photo", actual.Caption);
        }

        /// <summary>
        ///A test for DiscoverMetadata
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Content\TestBmpImage.bmp")]
        public void DiscoverMetadataImageBmpTest()
        {
            string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestBmpImage.bmp");
            var actual = ContentMetadataLoader.DiscoverMetadata(filepath);
            Assert.IsNull(actual.Caption, "Caption is not saved, and thus should be null");
        }

        /// <summary>
        ///A test for DiscoverMetadata
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Content\TestPngImage.png")]
        public void DiscoverMetadataImagePngTest()
        {
            string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestPngImage.png");
            var actual = ContentMetadataLoader.DiscoverMetadata(filepath);
            Assert.IsNull(actual.Caption, "Caption is not saved, and thus should be null");
        }

        /// <summary>
        ///A test for DiscoverMetadata
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Content\TestTifImage.tif")]
        public void DiscoverMetadataImageTifTest()
        {
            string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestTifImage.tif");
            var actual = ContentMetadataLoader.DiscoverMetadata(filepath);
            Assert.AreEqual("Test Tif File", actual.Caption);
        }

        /// <summary>
        ///A test for DiscoverMetadata
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Content\TestVideo.mov")]
        public void DiscoverMetadataVideoTest()
        {
            string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestVideo.mov");
            var actual = ContentMetadataLoader.DiscoverMetadata(filepath);
            Assert.AreEqual("This is a test Video", actual.Caption, "Caption");
            Assert.AreEqual(false, actual.IsHidden, "IsHidden");
            Assert.AreEqual(true, actual.IsVideo, "IsVideo Flag");
            Assert.AreEqual(1, actual.Keywords.Length, "Count of Keywords");
            Assert.AreEqual("Activity", actual.Keywords[0], "Keywords");
            Assert.AreEqual(new TimeSpan(0, 0, 1), actual.VideoLength, "Video Length is not 1 second");
        }

        /// <summary>
        ///A test for DiscoverMetadata
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Content\TestVideoMp4.mp4")]
        public void DiscoverMetadataVideoMp4Test()
        {
            string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestVideoMp4.mp4");
            var actual = ContentMetadataLoader.DiscoverMetadata(filepath);
            Assert.AreNotEqual(new TimeSpan(0, 0, 0), actual.VideoLength, "Video Length is returning as 0");
            Assert.AreEqual(true, actual.IsVideo, "IsVideo Flag");
            Assert.AreEqual("Test Mp4 Video", actual.Caption, "Caption");
        }

        /// <summary>
        ///A test for DiscoverMetadata
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Content\TestVideoMpg.mpg")]
        public void DiscoverMetadataVideoMpgTest()
        {
            string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestVideoMpg.mpg");
            var actual = ContentMetadataLoader.DiscoverMetadata(filepath);
            Assert.AreNotEqual(new TimeSpan(0, 0, 0), actual.VideoLength, "Video Length is returning as 0");
            Assert.AreEqual(true, actual.IsVideo, "IsVideo Flag");
            Assert.IsNull(actual.Caption, "Caption will not save, and will be null");
        }

        /// <summary>
        ///A test for DiscoverMetadata
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Content\TestVideoWmv.wmv")]
        public void DiscoverMetadataVideoWmvTest()
        {
            string filepath = System.IO.Path.Combine(this.GetDeploymentDirectory(), "TestVideoWmv.Wmv");
            var actual = ContentMetadataLoader.DiscoverMetadata(filepath);
            Assert.AreNotEqual(new TimeSpan(0, 0, 0), actual.VideoLength, "Video Length is returning as 0");
            Assert.AreEqual(true, actual.IsVideo, "IsVideo Flag");
            Assert.AreEqual("Test Wmv Video", actual.Caption, "Caption");
        }
    }
}
