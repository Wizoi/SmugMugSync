using SmugMugCore.Net.Data20;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Xmp;

namespace SmugMugCore.Net.Service20;

public partial class ContentMetadataService
{
    [GeneratedRegex("[;,]")]
    private partial Regex TagSplitRegex();

    public IFileSystem FileSystem = new FileSystem();

    public ContentMetadataService()   
    {}

    public virtual async Task<FileMetaContent> DiscoverMetadata(string filepath)
    {
        FileMetaContent content;
        if (this.FileSystem.File.Exists(filepath))
        {
            try
            {
                content = this.GetMetadataPropertiesWithMetadataExtractorLibrary(filepath);
            }
            catch (MetadataExtractor.ImageProcessingException)
            {
                // The file format is not read by MetadataExtractor (ie: WMV, MPG), so there is no metadata to read
                content = new FileMetaContent();
            }
        }
        else
            throw new ApplicationException("File " + filepath + " is not found.");

        content.IsVideo = IsVideoFile(filepath);
        if (content.IsVideo && (content.VideoLength == TimeSpan.Zero) && (System.IO.Path.GetExtension(filepath).ToUpperInvariant() == ".WMV"))
            content.VideoLength = GetWmvDuration(filepath);

        content.FileInfo = this.FileSystem.FileInfo.New(filepath);
        return content;
    }

    private static readonly HashSet<string> VideoExtensions = [".MOV", ".MP4", ".M4V", ".WMV", ".MPG", ".MPEG", ".MOD", ".AVI", ".3GP", ".MTS", ".M2TS"];

    private static bool IsVideoFile(string filepath)
    {
        return VideoExtensions.Contains(System.IO.Path.GetExtension(filepath).ToUpperInvariant());
    }

    /// <summary>
    /// Read the duration of a WMV (ASF) video, which MetadataExtractor does not read, from the ASF File Properties Object
    /// in the header: the play duration (100ns units) less the preroll (ms).
    /// </summary>
    /// <returns>The duration, or zero if it is not found</returns>
    private TimeSpan GetWmvDuration(string filepath)
    {
        byte[] filePropertiesGuid = [0xA1, 0xDC, 0xAB, 0x8C, 0x47, 0xA9, 0xCF, 0x11, 0x8E, 0xE4, 0x00, 0xC0, 0x0C, 0x20, 0x53, 0x65];

        // The header objects are at the start of the file
        byte[] header = new byte[64 * 1024];
        int length;
        using (var stream = this.FileSystem.File.OpenRead(filepath))
        {
            length = stream.Read(header, 0, header.Length);
        }

        // ASF Header Object: GUID (16), size (8), object count (4), reserved (2), then each object: GUID (16), size (8), data
        int offset = 30;
        while (offset + 88 <= length)
        {
            long objectSize = BitConverter.ToInt64(header, offset + 16);
            if (header.AsSpan(offset, 16).SequenceEqual(filePropertiesGuid))
            {
                long playDuration = BitConverter.ToInt64(header, offset + 64);
                long preroll = BitConverter.ToInt64(header, offset + 80);
                return TimeSpan.FromTicks(playDuration) - TimeSpan.FromMilliseconds(preroll);
            }
            if (objectSize < 24)
                break;
            offset += (int)Math.Min(objectSize, int.MaxValue);
        }
        return TimeSpan.Zero;
    }

    private FileMetaContent GetMetadataPropertiesWithMetadataExtractorLibrary(string filepath)
    {
        IEnumerable<MetadataExtractor.Directory> directories = ReadMetadataSkippingBadXmp(filepath);
        var content = new FileMetaContent();

        var xmpMeta = directories.OfType<XmpDirectory>().FirstOrDefault()?.XmpMeta;
        var iptcTags = directories.OfType<IptcDirectory>().FirstOrDefault();
        var exifTags = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
        var exifDetailTags = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
        var quickTimeTags = directories.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();

        // The XMP fields are read first (written by TagPup and Lightroom), then the Windows XP fields (Windows Photo Gallery), then IPTC
        content.Title = FirstNonEmpty(GetXmpLocalizedText(xmpMeta, "title"), GetWindowsXpText(exifTags, ExifIfd0Directory.TagWinTitle),
            iptcTags?.GetDescription(IptcDirectory.TagObjectName));

        // Windows Photo Gallery also writes the title to dc:description and the IPTC caption, so skip those when it is the title.
        string? NotTitle(string? caption) => (caption?.Trim() == content.Title?.Trim()) ? null : caption;
        content.Caption = FirstNonEmpty(NotTitle(GetXmpLocalizedText(xmpMeta, "description")), GetWindowsXpText(exifTags, ExifIfd0Directory.TagWinComment),
            NotTitle(iptcTags?.GetDescription(IptcDirectory.TagCaption)));

        // The EXIF ImageDescription is used as the title by Windows Photo Gallery, but other tools (ie: Lightroom)
        // write the caption there, so only use it as the title when it is not the caption.
        string? imageDescription = exifTags?.GetDescription(ExifIfd0Directory.TagImageDescription);
        if (string.IsNullOrWhiteSpace(content.Title) && (imageDescription?.Trim() != content.Caption?.Trim()))
            content.Title = FirstNonEmpty(imageDescription);

        string[] keywords = GetXmpKeywords(xmpMeta);
        if (keywords.Length == 0)
            keywords = SplitTagString(GetWindowsXpText(exifTags, ExifIfd0Directory.TagWinKeywords) ?? "");
        if (keywords.Length == 0)
            keywords = iptcTags?.GetStringArray(IptcDirectory.TagKeywords)?.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray() ?? [];
        content.Keywords = SplitTagString(string.Join(";", keywords));

        // Date Taken: XMP, then EXIF (photos), then the QuickTime header (videos)
        DateTime? dateTaken = GetXmpDateTaken(xmpMeta);
        if ((dateTaken == null) && (exifDetailTags?.TryGetDateTime(ExifSubIfdDirectory.TagDateTimeOriginal, out DateTime exifDetailDateTime) ?? false))
            dateTaken = exifDetailDateTime;
        if ((dateTaken == null) && (exifTags?.TryGetDateTime(ExifIfd0Directory.TagDateTime, out DateTime exifDateTime) ?? false))
            dateTaken = exifDateTime;
        if ((dateTaken == null) && (quickTimeTags?.TryGetDateTime(QuickTimeMovieHeaderDirectory.TagCreated, out DateTime qtDetailDateTime) ?? false))
            dateTaken = qtDetailDateTime;
        content.DateTaken = dateTaken ?? DateTime.MinValue;

        if ((exifTags?.TryGetUInt16(ExifIfd0Directory.TagOrientation, out ushort exifOrientation) ?? false))
            content.Orientation = (ImageContentOrientation)exifOrientation;

        if (quickTimeTags?.GetObject(QuickTimeMovieHeaderDirectory.TagDuration) is TimeSpan duration)
            content.VideoLength = duration;

        return content;
    }

    /// <summary>
    /// Read the keywords from the XMP fields: dc:subject (IPTC Core Keywords) and Lightroom's lr:hierarchicalSubject (ie: People|Kevin Idzi).
    /// Both are combined as Lightroom only writes the groupings (ie: People) to lr:hierarchicalSubject.
    /// </summary>
    /// <returns>Keywords using / for the hierarchy, empty if none found</returns>
    private static string[] GetXmpKeywords(XmpCore.IXmpMeta? xmpMeta)
    {
        return GetXmpArrayValues(xmpMeta, "http://purl.org/dc/elements/1.1/", "subject")
            .Concat(GetXmpArrayValues(xmpMeta, "http://ns.adobe.com/lightroom/1.0/", "hierarchicalSubject").Select(x => x.Replace('|', '/')))
            .ToArray();
    }

    /// <summary>
    /// Read the date taken from the XMP fields: exif:DateTimeOriginal, xmp:CreateDate, then photoshop:DateCreated.
    /// These are usually ISO dates (ie: 2024-11-28T14:48:39), but some tools write the EXIF format (ie: 2024:11:28 14:48:39).
    /// </summary>
    private static DateTime? GetXmpDateTaken(XmpCore.IXmpMeta? xmpMeta)
    {
        if (xmpMeta == null)
            return null;

        var dateFields = new[] {
            ("http://ns.adobe.com/exif/1.0/", "DateTimeOriginal"),
            ("http://ns.adobe.com/xap/1.0/", "CreateDate"),
            ("http://ns.adobe.com/photoshop/1.0/", "DateCreated") };
        foreach (var (schemaNs, propName) in dateFields)
        {
            if (!xmpMeta.DoesPropertyExist(schemaNs, propName))
                continue;

            string? value = xmpMeta.GetPropertyString(schemaNs, propName);
            if (DateTime.TryParseExact(value, "yyyy:MM:dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime exifFormatDate))
                return exifFormatDate;
            if (DateTimeOffset.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTimeOffset isoDate))
                return isoDate.DateTime;
        }
        return null;
    }

    /// <summary>
    /// Read the metadata of a file. Some JPEGs have an XMP block that is only zeros (ie: space reserved by an older tool),
    /// which MetadataExtractor fails on (OverflowException), so read those again without the XMP block.
    /// </summary>
    private static IEnumerable<MetadataExtractor.Directory> ReadMetadataSkippingBadXmp(string filepath)
    {
        try
        {
            return MetadataExtractor.ImageMetadataReader.ReadMetadata(filepath);
        }
        catch (Exception ex) when (ex is not MetadataExtractor.ImageProcessingException && IsJpeg(filepath))
        {
            var readersWithoutXmp = MetadataExtractor.Formats.Jpeg.JpegMetadataReader.AllReaders.Where(x => x is not XmpReader).ToList();
            return MetadataExtractor.Formats.Jpeg.JpegMetadataReader.ReadMetadata(filepath, readersWithoutXmp);
        }
    }

    private static bool IsJpeg(string filepath)
    {
        string extension = System.IO.Path.GetExtension(filepath).ToUpperInvariant();
        return extension == ".JPG" || extension == ".JPEG";
    }

    /// <summary>
    /// Read a Windows XP tag (ie: XPTitle, XPComment, XPKeywords). Windows writes these as UCS-2 bytes,
    /// but some tools write them as ASCII/UTF-8 text which would otherwise be decoded as garbled UCS-2.
    /// </summary>
    private static string? GetWindowsXpText(ExifIfd0Directory? exifTags, int tagType)
    {
        if (exifTags?.GetObject(tagType) is StringValue stringValue)
            return DecodeWindowsXpBytes(stringValue.Bytes);

        return exifTags?.GetDescription(tagType);
    }

    /// <summary>
    /// Decode the bytes of a Windows XP tag stored as an ASCII type. Some tools still write UCS-2 bytes there
    /// (ie: "P\0a\0r\0k\0"), which decoded as UTF-8 would leave NULs in the text and fail the upload headers.
    /// </summary>
    public static string DecodeWindowsXpBytes(byte[] bytes)
    {
        // A NUL before the terminator only happens with UCS-2 text
        int length = bytes.Length;
        while (length > 0 && bytes[length - 1] == 0)
            length--;
        bool isUcs2 = Array.IndexOf(bytes, (byte)0, 0, length) >= 0;

        if (isUcs2)
            return System.Text.Encoding.Unicode.GetString(bytes, 0, Math.Min(bytes.Length, length + (length % 2))).Replace("\0", "");

        return System.Text.Encoding.UTF8.GetString(bytes, 0, length);
    }

    /// <summary>
    /// Read the default language text of an XMP Dublin Core property (ie: dc:title, dc:description)
    /// </summary>
    private static string? GetXmpLocalizedText(XmpCore.IXmpMeta? xmpMeta, string propName)
    {
        const string dcSchemaNs = "http://purl.org/dc/elements/1.1/";
        if (xmpMeta == null || !xmpMeta.DoesPropertyExist(dcSchemaNs, propName))
            return null;

        return xmpMeta.GetLocalizedText(dcSchemaNs, propName, null, "x-default")?.Value;
    }

    /// <summary>
    /// Returns the first value that is not empty, or null if none are found.
    /// NULs are removed as some tools write an empty IPTC field as a single NUL (ie: Object Name = \0).
    /// </summary>
    private static string? FirstNonEmpty(params string?[] values)
    {
        return values.Select(x => x?.Replace("\0", "")).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
    }

    /// <summary>
    /// Read the values of an XMP array property (ie: dc:subject)
    /// </summary>
    private static string[] GetXmpArrayValues(XmpCore.IXmpMeta? xmpMeta, string schemaNs, string propName)
    {
        if (xmpMeta == null || !xmpMeta.DoesPropertyExist(schemaNs, propName))
            return [];

        var values = new List<string>();
        int itemCount = xmpMeta.CountArrayItems(schemaNs, propName);
        for (int i = 1; i <= itemCount; i++)
        {
            string? value = xmpMeta.GetArrayItem(schemaNs, propName, i)?.Value;
            if (!string.IsNullOrWhiteSpace(value))
                values.Add(value);
        }
        return values.ToArray();
    }

    /// <summary>
    /// Split a tag into the various elements for SmugMug's Usage
    /// </summary>
    /// <param name="tagList"></param>
    /// <returns></returns>
    private string[] SplitTagString(string tagList)
    {
        // Example: Family\Idzi\Kevin Idzi;Scenic\Wildlife -> two elements where the semi-colon splits
        var rawTagData = TagSplitRegex().Split(tagList.Replace("\0", "").Replace("; ", ";"));
        var tagData = new List<string>();

        // For each unique element, split the parts where we have hte \ adding more detail and grouping
        foreach (string tag in rawTagData)
        {
            if (tag.Length > 0)
            {
                if (!tag.Contains('/'))
                {
                    tagData.Add(this.CleanKeywordsForSmugMug(tag));
                }
                else
                {
                    var splitParts = tag.Split('/');
                    foreach (string part in splitParts)
                    {
                        tagData.Add(this.CleanKeywordsForSmugMug(part));
                    }
                }
            }
        }

        return tagData.Distinct().ToArray();
    }

    /// <summary>
    /// Checks a keyword vs the keyword list of an  image content for differences
    /// </summary>
    /// <param name="sourceImage"></param>
    /// <param name="tagList"></param>
    /// <returns>True if differences are found</returns>
    public bool AreKeywordsDifferent(FileMetaContent sourceImage, string? tagList)
    {
        if (String.IsNullOrWhiteSpace(tagList))
            tagList = string.Empty;

        string[] targetKeywordList;

        string targetKeywordString = tagList.Replace("; ", ";").Replace(", ", ",").Trim();
        if (targetKeywordString.Length == 0)
            targetKeywordList = [];
        else
            targetKeywordList = TagSplitRegex().Split(targetKeywordString).Select(x => x.Trim().Trim('"')).ToArray();

        var sourceKeywordList = sourceImage.Keywords.Select(x => x.Trim()).Where(x => x.Length > 0);

        if (sourceKeywordList.Except(targetKeywordList).Any())
            return true;

        if (targetKeywordList.Except(sourceKeywordList).Any())
            return true;

        return false;
    }

    /// <summary>
    /// Join the keywords to send to SmugMug. A single keyword ends with a ; as SmugMug otherwise splits it
    /// on the spaces (ie: John Doe -> John; Doe), with the ; SmugMug keeps it whole and quotes it ("John Doe").
    /// </summary>
    /// <param name="keywords">Keywords to join</param>
    /// <returns>Keywords separated by ;</returns>
    public static string JoinKeywordsForSmugMug(string[] keywords)
    {
        string keywordList = string.Join(";", keywords);
        return keywords.Length == 1 ? keywordList + ";" : keywordList;
    }

    /// <summary>
    /// SmugMugKeywords need to be uri friendly
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    private string CleanKeywordsForSmugMug(string keyword)
    {
        return keyword.Replace("&", "").Replace("-", "");
    }
}
