namespace TestSmugMugCoreNetAPI
{

    /// <summary>
    ///This is a test class for CouponServiceTest and is intended
    ///to contain all CouponServiceTest Unit Tests
    ///</summary>
    [Ignore("deprecated from SmugMugCore.NET due to lack of SmugMug 1.3 API Support")]
    [Obsolete()]
    public class CouponServiceTest
    {
        private static AlbumDetail _albumTest = null;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            var core = Utility.RetrieveSmugMugCore();
            _albumTest = Utility.CreateArbitraryTestAlbum(core, "TestAlbumForCoupons");
        }

        /// <summary>
        /// When class is done, remove the test album created at the beginning
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            var core = Utility.RetrieveSmugMugCore();
            Utility.RemoveArbitraryTestAlbum(core, "TestAlbumForCoupons");
        }
        

        /// <summary>
        ///A test for AddAlbumRestrictionToCoupon
        ///</summary>
        [TestMethod(), Ignore()]
        public void AddAlbumRestrictionToCouponTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            CouponService target = new CouponService(core); 
            string couponId = string.Empty; 
            string albumId = string.Empty; 
            bool expected = false; 
            bool actual;
            actual = target.AddAlbumRestrictionToCoupon(couponId, albumId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateCoupon
        ///</summary>
        [TestMethod(), Ignore()]
        public void CreateCouponTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            CouponService target = new CouponService(core);
            CouponInfo coupon = new CouponInfo();
            coupon.Code = "TestCoupon";
            coupon.Description = "Test Coupon Description";
            coupon.Discount = 1.15f;
            coupon.Title = "Test Coupon";

            CouponInfo actual;
            actual = target.CreateCoupon(coupon);
            Assert.AreEqual(actual.Title, coupon.Title);
            Assert.IsTrue(actual.CouponId > 0);
        }

        /// <summary>
        ///A test for GetCoupon
        ///</summary>
        [TestMethod(), Ignore()]
        public void GetCouponTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            CouponService target = new CouponService(core); 
            string couponId = string.Empty; 
            CouponInfo expected = null; 
            CouponInfo actual;
            actual = target.GetCoupon(couponId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RemoveAlbumRestrictionFromCoupon
        ///</summary>
        [TestMethod(), Ignore()]
        public void RemoveAlbumRestrictionFromCouponTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            CouponService target = new CouponService(core); 
            string couponId = string.Empty; 
            string albumId = string.Empty; 
            bool expected = false; 
            bool actual;
            actual = target.RemoveAlbumRestrictionFromCoupon(couponId, albumId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCouponList
        ///</summary>
        [TestMethod(), Ignore()]
        public void GetCouponListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            CouponService target = new CouponService(core); 
            string statusFilter = string.Empty; 
            string typeFilter = string.Empty; 
            CouponCore[] expected = null; 
            CouponCore[] actual;
            actual = target.GetCouponList(Array.Empty<string>(),  statusFilter, typeFilter);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCouponInfoList
        ///</summary>
        [TestMethod(), Ignore()]
        public void GetCouponInfoListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            CouponService target = new CouponService(core); 
            string statusFilter = string.Empty; 
            string typeFilter = string.Empty; 
            CouponInfo[] expected = null; 
            CouponInfo[] actual;
            actual = target.GetCouponInfoList(statusFilter, typeFilter);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for UpdateCoupon
        ///</summary>
        [TestMethod(), Ignore()]
        public void UpdateCouponTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            CouponService target = new CouponService(core); 
            CouponInfo coupon = null; 
            bool expected = false; 
            bool actual;
            actual = target.UpdateCoupon(coupon);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
