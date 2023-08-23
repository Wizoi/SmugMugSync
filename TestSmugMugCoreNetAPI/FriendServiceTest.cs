namespace TestSmugMugCoreNetAPI
{
    
    
    /// <summary>
    ///This is a test class for FriendServiceTest and is intended
    ///to contain all FriendServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FriendServiceTest
    {
        private string testFamily = "kilbournephoto";


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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for AddFriend
        ///</summary>
        [TestMethod()]
        public void AddRemoveFriendTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new FriendService(core);
            string nickName = testFamily;
            bool actual;

            // Clean up old data
            Utility.RemoveArbitraryTestFamilies(core, nickName);

            actual = target.AddFriend(nickName);
            Assert.AreEqual(actual, true);

            // Clean up when done
            Utility.RemoveArbitraryTestFriends(core, nickName);
        }

        /// <summary>
        ///A test for GetFriendList
        ///</summary>
        [TestMethod()]
        public void GetFriendListTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new FriendService(core);
            var actual = target.GetFriendList();

            // Not sure easy way to repeatedly verify this, manually tested this once and verified Serialization and API (2/11/2012)
        }

        /// <summary>
        ///A test for RemoveAll
        ///</summary>
        [TestMethod(), Ignore()]
        public void RemoveAllTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            var target = new FriendService(core);
            bool expected = true;
            bool actual = false;

            // Commented out to not cause harm, manually validated this works.
            // actual = target.RemoveAll();

            Assert.AreEqual(expected, actual);
        }

    }
}
