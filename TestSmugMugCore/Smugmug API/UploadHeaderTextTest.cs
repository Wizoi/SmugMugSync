using System.Text;
using SmugMugCore.Net.Service20;

namespace TestSmugMugCore.SmugMugAPI;

/// <summary>
/// Tests for the text sent in the upload headers (title, caption, keywords), these do not call SmugMug.
/// A NUL in a header is rejected by SmugMug with a 400 Bad Request, and a new line is rejected before sending.
/// </summary>
[TestClass()]
public class UploadHeaderTextTest
{
    [TestMethod()]
    public void DecodeWindowsXpBytes_Ascii()
    {
        Assert.AreEqual("Pepper in the snow", ContentMetadataService.DecodeWindowsXpBytes(Encoding.ASCII.GetBytes("Pepper in the snow\0")));
    }

    [TestMethod()]
    public void DecodeWindowsXpBytes_Utf8()
    {
        Assert.AreEqual("Café", ContentMetadataService.DecodeWindowsXpBytes(Encoding.UTF8.GetBytes("Café\0")));
    }

    [TestMethod()]
    public void DecodeWindowsXpBytes_Ucs2StoredAsAscii()
    {
        // Windows' UCS-2 text in a tag typed as ASCII would decode as "P\0a\0r\0k..." with UTF-8
        Assert.AreEqual("Parkrun 364", ContentMetadataService.DecodeWindowsXpBytes(Encoding.Unicode.GetBytes("Parkrun 364\0")));
        Assert.AreEqual("Parkrun 364", ContentMetadataService.DecodeWindowsXpBytes(Encoding.Unicode.GetBytes("Parkrun 364")));
    }

    [TestMethod()]
    public void DecodeWindowsXpBytes_Empty()
    {
        Assert.AreEqual("", ContentMetadataService.DecodeWindowsXpBytes([]));
        Assert.AreEqual("", ContentMetadataService.DecodeWindowsXpBytes([0, 0]));
    }

    [TestMethod()]
    public void CleanHeaderValue_RemovesControlCharacters()
    {
        Assert.AreEqual("", ImageUploaderService.CleanHeaderValue(null));
        Assert.AreEqual("Parkrun 364", ImageUploaderService.CleanHeaderValue("Parkrun 364"));
        Assert.AreEqual("Park", ImageUploaderService.CleanHeaderValue("P\0a\0r\0k\0"));
        Assert.AreEqual("Line one Line two", ImageUploaderService.CleanHeaderValue("Line one\r\nLine two\n"));
        Assert.AreEqual("Park run", ImageUploaderService.CleanHeaderValue("Park\trun"));
    }
}
