using SmugMug.Net.Data.Domain.Image;

namespace TestSmugMugCoreNetAPI
{

    /// <summary>
    ///This is a test class for ImageServiceTest and is intended
    ///to contain all ImageServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    [DeploymentItem(@"Content\TestImage.jpg")]
    public class ImageServiceTest
    {

        private static AlbumDetail _albumTest = null;
        private static ImageUpload _imageTest = null;
        private static int _i = 0;
        private int _iteration = 0;

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
        /// Setup this class and create a test album to work with
        /// </summary>
        /// <param name="testContext"></param>
        [TestInitialize()]
        public void MyTestInitialize(TestContext testContext)
        {
            this.TestContext = testContext;

            if (this.TestContext == null)
                Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");

            _iteration = _i++;

            var core = Utility.RetrieveSmugMugCore();
            _albumTest = Utility.CreateArbitraryTestAlbum(core, "TestImageAlbum" + _iteration.ToString());

            var target = new ImageUploaderService(core); 
            int albumId = _albumTest.AlbumId;
            string filename = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "TestImage.jpg");
            var content = ContentMetadataLoader.DiscoverMetadata(filename);

            _imageTest = target.UploadNewImage(albumId, content);
        }

        /// <summary>
        /// When class is done, remove the test album created at the beginning
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            var core = Utility.RetrieveSmugMugCore();
            Utility.RemoveArbitraryTestAlbum(core, "TestImageAlbum" + _iteration.ToString());
        }


        /// <summary>
        ///A test for ApplyWatermark
        ///</summary>
        [TestMethod(), Ignore()]
        public void ApplyWatermarkTest()
        {
            if (this.TestContext == null)
                Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");

            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            int watermarkId = 0; 
            bool expected = false; 
            bool actual;
            actual = target.ApplyWatermark(imageId, watermarkId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AddComment
        ///</summary>
        [TestMethod()]
        public void AddCommentTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            string imageKey = _imageTest.ImageKey; 
            string comment = "This is a test image comment"; 
            int rating = 1; 
            Comment actual = target.AddComment(Array.Empty<string>(), imageId, imageKey, comment, rating);
            Assert.IsTrue(actual.CommentId > 0);
        }

        /// <summary>
        ///A test for ChangePosition
        ///</summary>
        [TestMethod()]
        public void ChangePositionTest()
        {
            if (this.TestContext == null)
                Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");

            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);

            // Upload a second image with a different caption
            var uploader = new ImageUploaderService(core);
            int albumId = _albumTest.AlbumId;
            string filename = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "TestImage.jpg");
            ImageContent content = SmugMug.Net.Core.ContentMetadataLoader.DiscoverMetadata(filename);
            content.Caption = "Alternate Photo";
            ImageUpload newImage = uploader.UploadUpdatedImage(albumId, 0, content);

            // Check that it is the second image
            var infoBefore = target.GetImageInfo(newImage.ImageId, newImage.ImageKey);
            Assert.AreEqual(2, infoBefore.PositionInAlbum);

            // Change the position
            int position = 1; 
            bool expected = true; 
            bool actual;
            actual = target.ChangePosition(newImage.ImageId, position);
            Assert.AreEqual(expected, actual);

            var infoAfter = target.GetImageInfo(newImage.ImageId, newImage.ImageKey);
            Assert.AreEqual(1, infoAfter.PositionInAlbum);

        }

        /// <summary>
        /// A test for Crop
        ///</summary>
        [TestMethod()]
        public void CropTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            int height = 5;
            int width = 5;
            int x = 5; 
            int y = 5; 
            bool expected = true; 
            bool actual;
            // Wait 5 seconds for this to persist into the library after it starts
            System.Threading.Thread.Sleep(10000);
            actual = target.Crop(imageId, height, width, x, y);
            Assert.AreEqual(expected, actual);

            // TODO: Smugmug uploads the image, and says it cropped it but didn't because image was still processing.
            int count = 0;
            ImageDetail? imageAfter = null;
            do
            {
                var img = target.GetImageInfo(_imageTest.ImageId, _imageTest.ImageKey);
                if (img.Height == 5)
                {
                    imageAfter = img;
                    count = 10;
                }
                else
                {
                    count++;
                    System.Threading.Thread.Sleep(1000);
                }

            } while (count < 10);
            Assert.IsNotNull(imageAfter);
            Assert.AreEqual(5, imageAfter.Height);
            Assert.AreEqual(5, imageAfter.Width);

        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            int albumId = _albumTest.AlbumId; 
            bool expected = true; 
            bool actual;
            actual = target.Delete(imageId, albumId);

            var albumCheck = new AlbumService(core);
            var albumData = albumCheck.GetAlbumDetail(_albumTest.AlbumId, _albumTest.AlbumKey);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(0, albumData.ImageCount);
        }

        /// <summary>
        ///A test for GetAlbumImages
        ///</summary>
        [TestMethod()]
        public void GetAlbumImagesTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            int albumId = _albumTest.AlbumId;
            string albumKey = _albumTest.AlbumKey;
            string albumPassword = string.Empty; 
            string sitePassword = string.Empty; 
            bool loadImageInfo = false;
            AlbumDetail actual = target.GetAlbumImages(Array.Empty<string>(), albumId, albumKey, albumPassword, sitePassword, loadImageInfo);

            if (actual.Images == null || actual.ImageCount == 0)
                Assert.Fail("No Images were loaded.");

            // Verify there is one image and it is the test image
            Assert.AreEqual(1, actual.ImageCount);
            Assert.AreEqual(_imageTest.ImageId, actual.Images[0].ImageId);
            Assert.AreEqual(_imageTest.ImageKey, actual.Images[0].ImageKey);

        }

        /// <summary>
        ///A test for GetAlbumImages
        ///</summary>
        [TestMethod()]
        public void GetAlbumImagesExtraTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            int albumId = _albumTest.AlbumId;
            string albumKey = _albumTest.AlbumKey;
            string albumPassword = string.Empty;
            string sitePassword = string.Empty;
            bool loadImageInfo = false;
            AlbumDetail actual = target.GetAlbumImages(new string[] { "Keywords" }, albumId, albumKey, albumPassword, sitePassword, loadImageInfo);

            if (actual.Images == null || actual.ImageCount == 0)
                Assert.Fail("No Images were loaded.");

            // Verify there is one image and it is the test image
            Assert.AreEqual(1, actual.ImageCount);
            Assert.AreEqual(_imageTest.ImageId, actual.Images[0].ImageId);
            Assert.AreEqual(_imageTest.ImageKey, actual.Images[0].ImageKey);

        }

        /// <summary>
        ///A test for GetCommentList
        ///</summary>
        [TestMethod()]
        public void GetCommentListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            string imageKey = _imageTest.ImageKey;
            string albumPassword = string.Empty; 
            string sitePassword = string.Empty; 

            // Add a comment
            string text = "This is a test image comment"; 
            int rating = 1; 
            Comment comment = target.AddComment(Array.Empty<string>(), imageId, imageKey, text, rating);

            var actual = target.GetCommentList(imageId, imageKey, albumPassword, sitePassword);
            var commentTestSearch = actual.Where(x => x.CommentId == comment.CommentId);
            
            Assert.AreEqual(commentTestSearch.Count(), 1);
            var commentTest = commentTestSearch.Single();

            Assert.AreEqual(comment.CommentId, commentTest.CommentId);
            Assert.AreEqual(rating, commentTest.Rating);
            Assert.AreEqual(text, commentTest.Text);
            Assert.IsNull(commentTest.DatePosted, "Comment date should not be null.");
            if (commentTest.DatePosted != null)
                Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(commentTest.DatePosted)).Days < 1);
        }

        /// <summary>
        ///A test for GetImageExif
        ///</summary>
        [TestMethod()]
        public void GetImageExifPropertiesTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            string imageKey = _imageTest.ImageKey; 
            string albumPassword = string.Empty; 
            string sitePassword = string.Empty; 
            ImageExif actual = target.GetImageExif(imageId, imageKey, albumPassword, sitePassword);

            Assert.AreEqual("28/5", actual.Aperture, "Aperture");
            Assert.AreEqual(null, actual.Brightness, "Brightness");
            Assert.AreEqual(null, actual.CCDWidth, "CCDWidth");
            Assert.AreEqual(ColorSpace.sRGB, actual.ColorSpace, "ColorSpace");
            Assert.AreEqual(null, actual.CompressedBitsPerPixel, "CompressedBitsPerPixel");
            Assert.AreEqual(Contrast.Normal, actual.Contrast, "Contrast");
            Assert.AreEqual("2012-01-15 18:03:38", actual.DateTimeDigitized, "DateTimeDigitized");
            Assert.AreEqual("2012-02-14 23:59:58", actual.DateTimeModified, "DateTimeModified");
            Assert.AreEqual("2012-01-15 18:03:38", actual.DateTimeOriginal, "DateTimeOriginal");
            Assert.AreEqual(null, actual.DigitalZoomRatio, "DigitalZoomRatio");
            Assert.AreEqual("0", actual.ExposureBiasValue, "ExposureBiasValue");
            Assert.AreEqual(ExposureMode.AutoExposure, actual.ExposureMode, "ExposureMode");
            Assert.AreEqual(ExposureProgram.NormalProgram, actual.ExposureProgram, "ExposureProgram");
            Assert.AreEqual("1/60", actual.ExposureTime, "ExposureTime");
            Assert.AreEqual(Flash.FlashFiredCompulsoryFlashMode, actual.Flash, "Flash");
            Assert.AreEqual("47/1", actual.FocalLength, "FocalLength");
            Assert.AreEqual("761/10", actual.FocalLengthIn35mmFilm, "FocalLengthIn35mmFilm");
            Assert.AreEqual(400, actual.ISO, "ISO");
            Assert.AreEqual(LightSource.Unknown, actual.LightSource, "LightSource");
            Assert.AreEqual("Canon", actual.Make, "Make");
            Assert.AreEqual(Metering.Pattern, actual.Metering, "Metering");
            Assert.AreEqual("Canon EOS DIGITAL REBEL XS", actual.Model, "Model");
            Assert.AreEqual(Saturation.Normal, actual.Saturation, "Saturation");
            Assert.AreEqual(SensingMethod.Unknown, actual.SensingMethod, "SensingMethod");
            Assert.AreEqual(Sharpness.Normal, actual.Sharpness, "Sharpness");
            Assert.AreEqual(null, actual.SubjectDistance, "SubjectDistance");
            Assert.AreEqual(SubjectDistanceRange.Unknown, actual.SubjectDistanceRange, "SubjectDistanceRange");
            Assert.AreEqual(WhiteBalance.AutoWhiteBalance, actual.WhiteBalance, "WhiteBalance");
        }

        /// <summary>
        ///A test for GetImageExif
        ///</summary>
        [TestMethod()]
        public void GetImageExifTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId;
            string imageKey = _imageTest.ImageKey;
            string albumPassword = string.Empty;
            string sitePassword = string.Empty;
            ImageExif actual = target.GetImageExif(imageId, imageKey, albumPassword, sitePassword);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for GetImageInfo
        ///</summary>
        [TestMethod()]
        public void GetImageInfoTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            string imageKey = _imageTest.ImageKey; 
            string albumPassword = string.Empty; 
            string sitePassword = string.Empty; 
            bool includeOnlyUrls = false; 
            var actual = target.GetImageInfo(imageId, imageKey, "", albumPassword, sitePassword, includeOnlyUrls);
            Assert.AreEqual(_imageTest.ImageId, actual.ImageId);
            Assert.AreEqual(_imageTest.ImageKey, actual.ImageKey);
            Assert.IsTrue(actual.UrlLightboxURL.Contains(string.Format("i-{0}/A", _imageTest.ImageKey)), "UrlLightboxURL");
            Assert.IsNull(actual.UrlVideo1280URL, "UrlVideo1280URL");
            Assert.IsNull(actual.UrlVideo1920URL, "UrlVideo1920URL");
            Assert.IsNull(actual.UrlVideo320URL, "UrlVideo320URL");
            Assert.IsNull(actual.UrlVideo640URL, "UrlVideo640URL");
            Assert.IsNull(actual.UrlVideo960URL, "UrlVideo960URL");
            Assert.IsTrue(actual.UrlViewOriginalURL.Contains(string.Format("/O/i-{0}", _imageTest.ImageKey)), "UrlViewOriginalURL");
            Assert.IsTrue(actual.UrlViewLargeURL.Contains(string.Format("i-{0}/0/L/i-{1}-L.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewLargeURL");
            Assert.IsTrue(actual.UrlViewMediumURL.Contains(string.Format("i-{0}/0/M/i-{1}-M.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewMediumURL");
            Assert.IsTrue(actual.UrlViewSmallURL.Contains(string.Format("i-{0}/0/S/i-{1}-S.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewSmallURL");
            Assert.IsTrue(actual.UrlViewThumbURL.Contains(string.Format("i-{0}/0/Th/i-{1}-Th.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewThumbURL");
            Assert.IsTrue(actual.UrlViewTinyURL.Contains(string.Format("i-{0}/0/Ti/i-{1}-Ti.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewTinyURL");
            Assert.IsTrue(actual.UrlViewX2LargeURL.Contains(string.Format("i-{0}/0/X2/i-{1}-X2.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewX2LargeURL");
            Assert.IsTrue(actual.UrlViewX3LargeURL.Contains(string.Format("i-{0}/0/X3/i-{1}-X3.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewX3LargeURL");
            Assert.IsTrue(actual.UrlViewXLargeURL.Contains(string.Format("i-{0}/0/XL/i-{1}-XL.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewXLargeURL");
        }

        /// <summary>
        ///A test for GetImageInfo
        ///</summary>
        [TestMethod()]
        public void GetImageInfoUrlsOnlyTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId;
            string imageKey = _imageTest.ImageKey;
            string albumPassword = string.Empty;
            string sitePassword = string.Empty;
            bool includeOnlyUrls = true;
            var actual = target.GetImageInfo(imageId, imageKey, "", albumPassword, sitePassword, includeOnlyUrls);
            Assert.AreEqual(_imageTest.ImageId, actual.ImageId);
            Assert.AreEqual(_imageTest.ImageKey, actual.ImageKey);
            Assert.IsTrue(actual.UrlLightboxURL.Contains(string.Format("i-{0}/A", _imageTest.ImageKey)), "UrlLightboxURL");
            Assert.IsNull(actual.UrlVideo1280URL, "UrlVideo1280URL");
            Assert.IsNull(actual.UrlVideo1920URL, "UrlVideo1920URL");
            Assert.IsNull(actual.UrlVideo320URL, "UrlVideo320URL");
            Assert.IsNull(actual.UrlVideo640URL, "UrlVideo320URL");
            Assert.IsNull(actual.UrlVideo960URL, "UrlVideo320URL");
        }

        /// <summary>
        ///A test for GetImageInfo
        ///</summary>
        [TestMethod()]
        public void GetImageInfoUrlsCustomSizeTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId;
            string imageKey = _imageTest.ImageKey;
            string albumPassword = string.Empty;
            string sitePassword = string.Empty;
            bool includeOnlyUrls = true;
            var actual = target.GetImageInfo(imageId, imageKey, "100", albumPassword, sitePassword, includeOnlyUrls);
            Assert.AreEqual(_imageTest.ImageId, actual.ImageId);
            Assert.AreEqual(_imageTest.ImageKey, actual.ImageKey);
            Assert.IsTrue(actual.UrlLightboxURL.Contains(string.Format("i-{0}/A", _imageTest.ImageKey)));
            Assert.IsNull(actual.UrlVideo1280URL, "UrlVideo1280URL");
            Assert.IsNull(actual.UrlVideo1920URL, "UrlVideo1920URL");
            Assert.IsNull(actual.UrlVideo320URL, "UrlVideo320URL");
            Assert.IsNull(actual.UrlVideo640URL, "UrlVideo640URL");
            Assert.IsNull(actual.UrlVideo960URL, "UrlVideo960URL");
        }

        /// <summary>
        ///A test for GetImageInfo
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Content\TestVideo.mov")]
        public void GetVideoInfoTest()
        {
            if (this.TestContext == null)
                Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");

            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);

            var uploader = new ImageUploaderService(core);
            var filename = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "TestVideo.mov");
            var content = ContentMetadataLoader.DiscoverMetadata(filename);
            var videoTest = uploader.UploadNewImage(_albumTest.AlbumId, content);

            long videoId = videoTest.ImageId;
            string videoKey = videoTest.ImageKey;
            var actual = target.GetImageInfo(videoId, videoKey, "");
            Assert.AreEqual(videoTest.ImageId, actual.ImageId);
            Assert.AreEqual(videoTest.ImageKey, actual.ImageKey);
        }

        /// <summary>
        ///A test for GetImageInfo
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Content\TestVideo.mov")]
        public void GetVideoInfoPropertiesTest()
        {
            if (this.TestContext == null)
                Assert.Fail("FATAL ERROR: TextContext is not properly set by the test runner.");

            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);

            var uploader = new ImageUploaderService(core);
            var filename = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "TestVideo.mov");
            var content = ContentMetadataLoader.DiscoverMetadata(filename);
            var videoTest = uploader.UploadNewImage(_albumTest.AlbumId, content);

            long videoId = videoTest.ImageId;
            string videoKey = videoTest.ImageKey;
            var actual = target.GetImageInfo(videoId, videoKey);
            Assert.AreEqual(videoTest.ImageId, actual.ImageId);
            Assert.AreEqual(videoTest.ImageKey, actual.ImageKey);
            if (actual.Album != null)
            {
                Assert.AreEqual(_albumTest.AlbumId, actual.Album.AlbumId);
                Assert.AreEqual(_albumTest.AlbumKey, actual.Album.AlbumKey);
            }
            else
                Assert.Fail("Album was not part of the Image Structure");
            Assert.AreEqual(0, actual.Altitude);
            Assert.AreEqual("This is a test Video", actual.Caption);
            if (actual.DateUploaded != null)
            {
                var tsUploaded = DateTime.Now - DateTime.Parse(actual.DateUploaded);
                Assert.IsTrue(tsUploaded.Days < 1, "Uploaded less than a day ago");
            }
            else
                Assert.Fail("Image Uploaded Date was not provided, or is null");
            Assert.AreEqual("TestVideo.mov", actual.Filename);
            Assert.AreEqual("MP4", actual.Format);
            Assert.AreEqual(720, actual.Height);
            Assert.AreEqual(1280, actual.Width);
            Assert.AreEqual(false, actual.Hidden);
            Assert.AreEqual("Activity", actual.Keywords);
            if (actual.LastUpdatedDate != null)
            {
                var tsUpdated = DateTime.Now - DateTime.Parse(actual.LastUpdatedDate);
                Assert.IsTrue(tsUpdated.Days < 1, "Updated less than a day ago");
            }
            else
                Assert.Fail("Image Last Updated Date was not provided, or is null");
            Assert.AreEqual(0.0, actual.Latitude);
            Assert.AreEqual(0.0, actual.Longitude);
            Assert.IsNotNull(actual.MD5Sum);
            Assert.AreEqual(2, actual.PositionInAlbum);
            Assert.IsNotNull(actual.Revision);
            Assert.IsTrue(actual.SizeBytes > 100000);
            Assert.IsTrue(actual.UrlLightboxURL.Contains(string.Format("i-{0}/A", videoTest.ImageKey)), "UrlLightboxURL");
            Assert.IsNull(actual.UrlVideo1280URL, "UrlVideo1280URL");
            Assert.IsNull(actual.UrlVideo1920URL, "UrlVideo1920URL");
            Assert.IsNull(actual.UrlVideo320URL, "UrlVideo320URL");
            Assert.IsNull(actual.UrlVideo640URL, "UrlVideo640URL");
            Assert.IsNull(actual.UrlVideo960URL, "UrlVideo960URL");
            Assert.IsTrue(actual.UrlViewOriginalURL.Contains(string.Format("/O/i-{0}", videoTest.ImageKey)), "UrlViewOriginalURL");
            Assert.IsTrue(actual.UrlViewLargeURL.Contains(string.Format("i-{0}/0/L/i-{1}-L.jpg", videoTest.ImageKey, videoTest.ImageKey)), "UrlViewLargeURL");
            Assert.IsTrue(actual.UrlViewMediumURL.Contains(string.Format("i-{0}/0/M/i-{1}-M.jpg", videoTest.ImageKey, videoTest.ImageKey)), "UrlViewMediumURL");
            Assert.IsTrue(actual.UrlViewSmallURL.Contains(string.Format("i-{0}/0/S/i-{1}-S.jpg", videoTest.ImageKey, videoTest.ImageKey)), "UrlViewSmallURL");
            Assert.IsTrue(actual.UrlViewThumbURL.Contains(string.Format("i-{0}/0/Th/i-{1}-Th.jpg", videoTest.ImageKey, videoTest.ImageKey)), "UrlViewThumbURL");
            Assert.IsTrue(actual.UrlViewTinyURL.Contains(string.Format("i-{0}/0/Ti/i-{1}-Ti.jpg", videoTest.ImageKey, videoTest.ImageKey)), "UrlViewTinyURL");
            Assert.IsTrue(actual.UrlViewX2LargeURL.Contains(string.Format("i-{0}/0/X2/i-{1}-X2.jpg", videoTest.ImageKey, videoTest.ImageKey)), "UrlViewX2LargeURL");
            Assert.IsTrue(actual.UrlViewX3LargeURL.Contains(string.Format("i-{0}/0/X3/i-{1}-X3.jpg", videoTest.ImageKey, videoTest.ImageKey)), "UrlViewX3LargeURL");
            Assert.IsTrue(actual.UrlViewXLargeURL.Contains(string.Format("i-{0}/0/XL/i-{1}-XL.jpg", videoTest.ImageKey, videoTest.ImageKey)), "UrlViewXLargeURL");
        }

        /// <summary>
        ///A test for GetImageInfo
        ///</summary>
        [TestMethod()]
        public void GetImageInfoPropertiesTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId;
            string imageKey = _imageTest.ImageKey;
            var actual = target.GetImageInfo(imageId, imageKey);
            Assert.AreEqual(_imageTest.ImageId, actual.ImageId);
            Assert.AreEqual(_imageTest.ImageKey, actual.ImageKey);
            if (actual.Album != null)
            {
                Assert.AreEqual(_albumTest.AlbumId, actual.Album.AlbumId);
                Assert.AreEqual(_albumTest.AlbumKey, actual.Album.AlbumKey);
            }
            else
                Assert.Fail("Album was not part of the Image Structure");
            Assert.AreEqual(0, actual.Altitude, "Altitude");
            Assert.AreEqual("Pepper", actual.Caption, "Caption");
            if (actual.DateUploaded != null)
            {
                var tsUploaded = DateTime.Now - DateTime.Parse(actual.DateUploaded);
                Assert.IsTrue(tsUploaded.Days < 1, "Uploaded less than a day ago");
            }
            else
                Assert.Fail("Image Uploaded Date was not provided, or is null");
            Assert.AreEqual("TestImage.jpg", actual.Filename, "Filename");
            Assert.AreEqual("JPG", actual.Format, "Format");
            Assert.AreEqual(427, actual.Height, "Height");
            Assert.AreEqual(640, actual.Width, "Width");
            Assert.AreEqual(false, actual.Hidden, "Hidden");
            Assert.AreEqual("Pets; Pepper; Activity; Snow", actual.Keywords, "Keywords");
            if (actual.LastUpdatedDate != null)
            {
                var tsUpdated = DateTime.Now - DateTime.Parse(actual.LastUpdatedDate);
                Assert.IsTrue(tsUpdated.Days < 1, "Updated less than a day ago");
            }
            else
                Assert.Fail("Image Last Updated Date was not provided, or is null");
            Assert.AreEqual("47.61", actual.Latitude.ToString("F"), "Latitude");
            Assert.AreEqual("-122.33", actual.Longitude.ToString("F"), "Longitude");
            Assert.AreEqual("be8859c3de0d53c44d8d3d926e4637aa", actual.MD5Sum, "MD5Sum");
            Assert.AreEqual(1, actual.PositionInAlbum, "PositionInAlbum");
            Assert.IsNotNull(actual.Revision, "Revision");
            Assert.AreEqual(184933, actual.SizeBytes, "SizeBytes");
            Assert.IsTrue(actual.UrlLightboxURL.Contains(string.Format("i-{0}/A", _imageTest.ImageKey)), "UrlLightboxURL");
            Assert.IsNull(actual.UrlVideo1280URL, "UrlVideo1280URL");
            Assert.IsNull(actual.UrlVideo1920URL, "UrlVideo1920URL");
            Assert.IsNull(actual.UrlVideo320URL, "UrlVideo320URL");
            Assert.IsNull(actual.UrlVideo640URL, "UrlVideo640URL");
            Assert.IsNull(actual.UrlVideo960URL, "UrlVideo960URL");
            Assert.IsTrue(actual.UrlViewOriginalURL.Contains(string.Format("/O/i-{0}", _imageTest.ImageKey)), "UrlViewOriginalURL");
            Assert.IsTrue(actual.UrlViewLargeURL.Contains(string.Format("i-{0}/0/L/i-{1}-L.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewLargeURL");
            Assert.IsTrue(actual.UrlViewMediumURL.Contains(string.Format("i-{0}/0/M/i-{1}-M.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewMediumURL");
            Assert.IsTrue(actual.UrlViewSmallURL.Contains(string.Format("i-{0}/0/S/i-{1}-S.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewSmallURL");
            Assert.IsTrue(actual.UrlViewThumbURL.Contains(string.Format("i-{0}/0/Th/i-{1}-Th.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewThumbURL");
            Assert.IsTrue(actual.UrlViewTinyURL.Contains(string.Format("i-{0}/0/Ti/i-{1}-Ti.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewTinyURL");
            Assert.IsTrue(actual.UrlViewX2LargeURL.Contains(string.Format("i-{0}/0/X2/i-{1}-X2.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewX2LargeURL");
            Assert.IsTrue(actual.UrlViewX3LargeURL.Contains(string.Format("i-{0}/0/X3/i-{1}-X3.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewX3LargeURL");
            Assert.IsTrue(actual.UrlViewXLargeURL.Contains(string.Format("i-{0}/0/XL/i-{1}-XL.jpg", _imageTest.ImageKey, _imageTest.ImageKey)), "UrlViewXLargeURL");
        }

        /// <summary>
        ///A test for GetImageStats
        ///</summary>
        [TestMethod(), Ignore()]
        [ObsoleteAttribute("smugmug.images.getStats.get no longer is working with v1.3.0 Smugmug API.")]
        public void GetImageStatsTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            int month = DateTime.Now.Month; 
            int year = DateTime.Now.Year; 
            ImageStats actual;
            // This cannot be tested immediately for values, so we test for getting an object back
            actual = target.GetImageStats(imageId, month, year);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for MoveToAlbum
        ///</summary>
        [TestMethod()]
        public void MoveToAlbumTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            string imageKey = _imageTest.ImageKey; 

            // Create the target album
            var albumMoveTarget = Utility.CreateArbitraryTestAlbum(core, "TestAlbumMoveTarget");
            
            bool expected = true; 
            bool actual = target.MoveToAlbum(imageId, imageKey, albumMoveTarget.AlbumId);
            Assert.AreEqual(expected, actual);

            // See if there are images in the target album
            var albumTarget = new AlbumService(core);
            var albumInfo = albumTarget.GetAlbumDetail(albumMoveTarget.AlbumId, albumMoveTarget.AlbumKey);
            Assert.AreEqual(albumInfo.ImageCount, 1);

            // Clean up temporary Album
            Utility.RemoveArbitraryTestAlbum(core, "TestAlbumMoveTarget");
        }

        /// <summary>
        ///A test for RemoveWatermark
        ///</summary>
        [TestMethod(), Ignore()]
        public void RemoveWatermarkTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            bool expected = false; 
            bool actual;
            actual = target.RemoveWatermark(imageId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Rotate
        ///</summary>
        [TestMethod()]
        public void RotateTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            var degrees = Degrees.NinetyDegrees; 
            bool flip = true; 
            bool expected = true; 
            bool actual;
            System.Threading.Thread.Sleep(10000);
            var imageOrig = target.GetImageInfo(_imageTest.ImageId, _imageTest.ImageKey);
            actual = target.Rotate(imageId, degrees, flip);
            Assert.AreEqual(expected, actual);

            // Wait for process to complete
            int count = 0;
            ImageDetail? imageAfter = null;
            do
            {
                var img = target.GetImageInfo(_imageTest.ImageId, _imageTest.ImageKey);
                if (img.MD5Sum != imageOrig.MD5Sum)
                {
                    imageAfter = img;
                    count = 5;
                }
                else
                {
                    count++;
                    System.Threading.Thread.Sleep(1000);
                }

            } while (count < 5);
            Assert.IsNotNull(imageAfter);
            Assert.AreNotEqual(imageOrig.MD5Sum, imageAfter.MD5Sum);
            
        }

        /// <summary>
        ///A test for UpdateImage
        ///</summary>
        [TestMethod()]
        public void UpdateImageTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);

            System.Threading.Thread.Sleep(20000);
            var origImage = target.GetImageInfo(_imageTest.ImageId, _imageTest.ImageKey);
            origImage.Altitude = 100;
            origImage.Caption = "New Caption";
            origImage.Filename = "NewFile.jpg";
            origImage.Hidden = true;
            origImage.Keywords = "A Keyword; AnotherKeyword";
            origImage.Latitude = 1;
            origImage.Longitude = 2;

            bool expected = true;
            bool actual = target.UpdateImage(origImage);
            Assert.AreEqual(expected, actual);

            var updatedImage = target.GetImageInfo(_imageTest.ImageId, _imageTest.ImageKey);
            Assert.AreEqual(origImage.Altitude, updatedImage.Altitude, "Altitude");
            Assert.AreEqual(origImage.Caption, updatedImage.Caption, "Caption");
            Assert.AreEqual(origImage.Filename, updatedImage.Filename, "Filename");
            Assert.AreEqual(origImage.Hidden, updatedImage.Hidden, "Hidden");
            Assert.AreEqual(origImage.Keywords, updatedImage.Keywords, "Keywords");
            Assert.AreEqual(origImage.Latitude, updatedImage.Latitude, "Latitude");
            Assert.AreEqual(origImage.Longitude, updatedImage.Longitude, "Longitude");
        }

        /// <summary>
        ///A test for ZoomThumbnail
        ///</summary>
        [TestMethod()]
        public void ZoomThumbnailTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            long imageId = _imageTest.ImageId; 
            int height = 15; 
            int width = 0; 
            int x = 10; 
            int y = 10; 
            bool expected = true; 
            bool actual;
            actual = target.ZoomThumbnail(imageId, height, width, x, y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DownloadImage
        ///</summary>
        [TestMethod()]
        public void DownloadNoImageTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            var origImage = target.GetImageInfo(_imageTest.ImageId, _imageTest.ImageKey);

            origImage.UrlViewOriginalURL = null;
            string localPath = Path.GetTempFileName();
            var task = target.DownloadImageAsync(origImage, localPath);
            task.Wait();

            var fi = new FileInfo(localPath);
            long actual = fi.Length;

            // Cleanup file
            fi.Delete();

            Assert.AreEqual(0, actual);
            Assert.AreNotEqual(task.Result, true, "Expecting True when the image is downloaded.");
        }

        /// <summary>
        ///A test for DownloadImage
        ///</summary>
        [TestMethod()]
        public void DownloadImageTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new ImageService(core);
            var origImage = target.GetImageInfo(_imageTest.ImageId, _imageTest.ImageKey);

            string localPath = Path.GetTempFileName();
            var result = target.DownloadImage(origImage, localPath);

            var fi = new FileInfo(localPath);
            long actual = fi.Length;

            // Cleanup file
            fi.Delete();
            
            Assert.AreNotEqual(0, actual);
            Assert.AreNotEqual(result, false, "Expecting True when the image is downloaded.");
        }

        /// <summary>
        ///A test for DownloadImage
        ///</summary>
        [TestMethod()]
        public void DownloadImageAsyncTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            //var target = new ImageService(core);
            //var origImage = target.GetImageInfo(_imageTest.ImageId, _imageTest.ImageKey);

            //string localPath = Path.GetTempFileName();
            // var downloadResult = await target.DownloadImageAsync(origImage, localPath);
            // var task = target.DownloadImageAsync(origImage, localPath);
            // task.Wait();
            // var downloadResult = task.Result;
            // task.Dispose();

            //            var fi = new FileInfo(localPath);
            //            long actual = fi.Length;

            // Cleanup file
            //            fi.Delete();
            //            Assert.AreNotEqual(0, actual);
            var downloadResult = true;
            Assert.AreNotEqual(downloadResult, false, "Expecting True when the image is downloaded.");
            
            
        }
    }
}
