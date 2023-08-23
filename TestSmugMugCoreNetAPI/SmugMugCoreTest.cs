namespace TestSmugMugCoreNetAPI
{
    /// <summary>
    ///This is a test class for SmugMugCoreTest and is intended
    ///to contain all SmugMugCoreTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SmugMugCoreTest
    {

        /// <summary>
        ///A test for PingService
        ///</summary>
        [TestMethod()]
        public void PingServiceTest()
        {
            var core = Utility.RetrieveSmugMugCore();
            bool expected = true;
            bool actual = core.PingService();
            Assert.AreEqual(expected, actual);
        }
    }
}
