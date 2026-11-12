using SmugMugCore.Net.Service20;

namespace TestSmugMugCore.SmugMugAPI;

/// <summary>
/// Tests for reading the metadata from the files, these do not call SmugMug.
/// </summary>
[TestClass()]
public class ContentMetadataServiceTest
{
    private static string ContentFile(string fileName) => System.IO.Path.Combine(AppContext.BaseDirectory, "Content", fileName);

    [TestMethod()]
    public async Task DiscoverMetadata_XmpBeforeWindowsFields()
    {
        // The XMP fields (written by TagPup) are different from the Windows XP fields, and are read first
        var content = await new ContentMetadataService().DiscoverMetadata(ContentFile("TestImageWithXmpAndXPFields.jpg"));
        Assert.AreEqual("Pepper from XMP", content.Title, "Title");
        Assert.AreEqual("Pepper sledding in the snow from XMP", content.Caption, "Caption");
        CollectionAssert.AreEqual(new[] { "Pets", "Pepper", "Activity", "Sledding" }, content.Keywords, "Keywords");
        Assert.AreEqual(new DateTime(2013, 2, 3, 4, 5, 6), content.DateTaken, "DateTaken");
        Assert.IsFalse(content.IsVideo, "IsVideo");
    }

    [TestMethod()]
    public async Task DiscoverMetadata_WindowsFieldsWithoutXmpText()
    {
        // The XMP has keywords and a date (xmp:CreateDate), but no title or caption, so those come from the Windows XP fields
        var content = await new ContentMetadataService().DiscoverMetadata(ContentFile("TestImageWithXPTitleCaption.jpg"));
        Assert.AreEqual("Pepper", content.Title, "Title");
        Assert.AreEqual("Pepper in the snow", content.Caption, "Caption");
        CollectionAssert.AreEqual(new[] { "Pets", "Pepper", "Activity", "Snow" }, content.Keywords, "Keywords");
        Assert.AreEqual(new DateTime(2012, 1, 15, 18, 3, 38), content.DateTaken, "DateTaken");
    }

    [TestMethod()]
    public async Task DiscoverMetadata_QuickTimeVideos()
    {
        var mov = await new ContentMetadataService().DiscoverMetadata(ContentFile("TestVideo.MOV"));
        Assert.IsTrue(mov.IsVideo, "MOV IsVideo");
        Assert.AreEqual(9, (int)mov.VideoLength.TotalSeconds, "MOV VideoLength");
        Assert.AreEqual(2016, mov.DateTaken.Year, "MOV DateTaken");

        var mp4 = await new ContentMetadataService().DiscoverMetadata(ContentFile("TestVideoMp4.mp4"));
        Assert.IsTrue(mp4.IsVideo, "MP4 IsVideo");
        Assert.AreEqual(5, (int)mp4.VideoLength.TotalSeconds, "MP4 VideoLength");
        Assert.AreEqual(2020, mp4.DateTaken.Year, "MP4 DateTaken");
    }

    [TestMethod()]
    public async Task DiscoverMetadata_VideosNotReadByMetadataExtractor()
    {
        // WMV and MPG are not read by MetadataExtractor, the WMV duration is read from the ASF header
        var wmv = await new ContentMetadataService().DiscoverMetadata(ContentFile("TestVideoWmv.wmv"));
        Assert.IsTrue(wmv.IsVideo, "WMV IsVideo");
        Assert.IsTrue(wmv.VideoLength > TimeSpan.Zero, "WMV VideoLength");

        var mpg = await new ContentMetadataService().DiscoverMetadata(ContentFile("TestVideoMpg.MPG"));
        Assert.IsTrue(mpg.IsVideo, "MPG IsVideo");
        Assert.AreEqual(TimeSpan.Zero, mpg.VideoLength, "MPG VideoLength is not read");
    }
}
