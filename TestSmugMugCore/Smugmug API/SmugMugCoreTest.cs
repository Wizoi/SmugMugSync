namespace TestSmugMugCore.SmugMugAPI;

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
    public async Task PingServiceTest()
    {
        var core = Utility.RetrieveSmugMugCore20();
        bool expected = true;
        bool actual = await core.PingService();
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// The services are first created by the parallel uploads, which corrupted the service catalog when it was not thread safe
    /// </summary>
    [TestMethod()]
    public async Task Services_CreatedFromParallelThreads()
    {
        for (int run = 0; run < 20; run++)
        {
            var core = new SmugMugCore.Net.Core20.SmugMugCore();
            var services = await Task.WhenAll(Enumerable.Range(0, 32).Select(i => Task.Run<object>(() => (i % 4) switch
            {
                0 => core.AlbumService,
                1 => core.AlbumImageService,
                2 => core.ImageUploaderService,
                _ => core.ContentMetadataService
            })));

            Assert.AreEqual(4, services.Distinct().Count(), "Expected one instance of each service");
        }
    }
}
