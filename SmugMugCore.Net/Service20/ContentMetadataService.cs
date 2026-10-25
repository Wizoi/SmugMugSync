using SmugMugCore.Net.Data20;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
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
                content = this.GetMetadataPropertiesFromShell(filepath); 
            }
        }
        else
            throw new ApplicationException("File " + filepath + " is not found.");

        content.FileInfo = this.FileSystem.FileInfo.New(filepath);
        return content;
    }

    private FileMetaContent GetMetadataPropertiesFromShell(string filepath)
    {
        var content = new FileMetaContent();
        PopulateMetadataPropertiesFromShell(filepath, content);
        return content;
    }        

    private void PopulateMetadataPropertiesFromShell(string filepath, FileMetaContent content)
    {
        if (ShellObject.IsPlatformSupported)
        {
            ShellProperties? propertyCollection = null;

            // Dispose for the file is unstable for windows as this shell object is not f
            using var file = ShellObject.FromParsingName(filepath);
            propertyCollection = file.Properties;
            if (propertyCollection != null)
            {
                IShellProperty propertyTitle = propertyCollection.GetProperty("System.Title");
                if (propertyTitle != null)
                {
                    content.Title = (string)propertyTitle.ValueAsObject;
                }

                IShellProperty propertyDuration = propertyCollection.GetProperty("System.Media.Duration");
                if (propertyDuration != null)
                {
                    content.IsVideo = true;
                    long duration = Convert.ToInt64(propertyDuration.ValueAsObject);
                    var ts = new TimeSpan(duration);
                    content.VideoLength = ts;
                }

                IShellProperty propertyDateTaken = propertyCollection.GetProperty("System.Document.DateCreated");
                if (propertyDateTaken != null)
                {
                    content.DateTaken = (DateTime)propertyDateTaken.ValueAsObject;
                }
                else
                {
                    content.DateTaken = DateTime.MinValue;
                }

                content.Keywords = [];
                IShellProperty propertyTags = propertyCollection.GetProperty("System.Keywords");
                if (propertyTags != null)
                {
                    string[] dataTag = (string[])propertyTags.ValueAsObject;
                    if (dataTag != null && dataTag.Any())
                    {
                        string keywordString = string.Join(";", (string[])propertyTags.ValueAsObject);
                        content.Keywords = SplitTagString(keywordString);
                    }
                }
            }
        }
    }

    private FileMetaContent GetMetadataPropertiesWithMetadataExtractorLibrary(string filepath)
    {
        IEnumerable<MetadataExtractor.Directory> directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(filepath);
        var content = new FileMetaContent();
        string? imageDescription = null;

        var exifDirTags = directories.OfType<MetadataExtractor.Formats.Exif.ExifIfd0Directory>();
        if (exifDirTags.Any())
        {
            var exifTags = exifDirTags.First();
            if (exifTags.HasTagName(ExifIfd0Directory.TagDateTime))
            {
                if (exifTags.TryGetDateTime(ExifIfd0Directory.TagDateTime, out DateTime exifDateTime))
                    content.DateTaken = exifDateTime;
            }
            if (exifTags.HasTagName(ExifIfd0Directory.TagWinTitle))
                content.Title = GetWindowsXpText(exifTags, ExifIfd0Directory.TagWinTitle);
            if (exifTags.HasTagName(ExifIfd0Directory.TagImageDescription))
                imageDescription = exifTags.GetDescription(ExifIfd0Directory.TagImageDescription);
            if (exifTags.HasTagName(ExifIfd0Directory.TagWinComment))
                content.Caption = GetWindowsXpText(exifTags, ExifIfd0Directory.TagWinComment);
            if (exifTags.HasTagName(ExifIfd0Directory.TagWinKeywords))
                content.Keywords = SplitTagString(GetWindowsXpText(exifTags, ExifIfd0Directory.TagWinKeywords) ?? "");

            bool foundOrientation = exifTags.TryGetUInt16(ExifIfd0Directory.TagOrientation, out ushort exifOrientation);
            if (foundOrientation)
            {
                content.Orientation = (ImageContentOrientation)exifOrientation;
            }
        }

        // Fallback to the standard keyword fields if the Windows keywords (XPKeywords) are not written
        if (content.Keywords.Length == 0)
        {
            var standardKeywords = GetStandardKeywords(directories);
            if (standardKeywords.Length > 0)
                content.Keywords = SplitTagString(string.Join(";", standardKeywords));
        }

        // Fallback to the standard title/caption fields if the Windows ones (XPTitle, XPComment) are not written
        var xmpMeta = directories.OfType<XmpDirectory>().FirstOrDefault()?.XmpMeta;
        var iptcTags = directories.OfType<IptcDirectory>().FirstOrDefault();
        if (string.IsNullOrWhiteSpace(content.Title))
            content.Title = FirstNonEmpty(GetXmpLocalizedText(xmpMeta, "title"), iptcTags?.GetDescription(IptcDirectory.TagObjectName));

        // Windows Photo Gallery also writes the title to dc:description and the IPTC caption, so skip those when it is the title.
        if (string.IsNullOrWhiteSpace(content.Caption))
        {
            string? standardCaption = FirstNonEmpty(GetXmpLocalizedText(xmpMeta, "description"), iptcTags?.GetDescription(IptcDirectory.TagCaption));
            if (standardCaption?.Trim() != content.Title?.Trim())
                content.Caption = standardCaption;
        }

        // The EXIF ImageDescription is used as the title by Windows Photo Gallery, but other tools (ie: Lightroom)
        // write the caption there, so only use it as the title when it is not the caption.
        if (string.IsNullOrWhiteSpace(content.Title) && (imageDescription?.Trim() != content.Caption?.Trim()))
            content.Title = FirstNonEmpty(imageDescription);

        // Second level of detail sometimes provided
        var exifDetailDirTags = directories.OfType<MetadataExtractor.Formats.Exif.ExifSubIfdDirectory>();
        if (exifDetailDirTags.Any())
        {
            var exifDetailTags = exifDetailDirTags.First();
            if (exifDetailTags.HasTagName(ExifSubIfdDirectory.TagDateTimeOriginal))
                if (exifDetailTags.TryGetDateTime(ExifSubIfdDirectory.TagDateTimeOriginal, out DateTime exifDetailDateTime))
                    content.DateTaken = exifDetailDateTime;
        }

        var quickTimeMovieTags = directories.OfType<MetadataExtractor.Formats.QuickTime.QuickTimeMovieHeaderDirectory>();
        if (quickTimeMovieTags.Any())
        {
            var quickTimeTags = quickTimeMovieTags.First();
            if (quickTimeTags.HasTagName(QuickTimeMovieHeaderDirectory.TagCreated))
                if (quickTimeTags.TryGetDateTime(QuickTimeMovieHeaderDirectory.TagCreated, out DateTime qtDetailDateTime))
                    content.DateTaken = qtDetailDateTime;

            PopulateMetadataPropertiesFromShell(filepath, content);
        }


        return content;
    }

    /// <summary>
    /// Read the keywords from the standard fields and combine them, in order:
    /// XMP dc:subject (IPTC Core Keywords), IPTC IIM Keywords, then Lightroom's XMP lr:hierarchicalSubject (ie: People|Kevin Idzi).
    /// All are combined as Lightroom only writes the groupings (ie: People) to lr:hierarchicalSubject.
    /// </summary>
    /// <param name="directories">Metadata read from the file</param>
    /// <returns>Keywords using / for the hierarchy, empty if none found</returns>
    private static string[] GetStandardKeywords(IEnumerable<MetadataExtractor.Directory> directories)
    {
        var xmpMeta = directories.OfType<XmpDirectory>().FirstOrDefault()?.XmpMeta;
        var iptcTags = directories.OfType<IptcDirectory>().FirstOrDefault();

        return GetXmpArrayValues(xmpMeta, "http://purl.org/dc/elements/1.1/", "subject")
            .Concat(iptcTags?.GetStringArray(IptcDirectory.TagKeywords)?.Where(x => !string.IsNullOrWhiteSpace(x)) ?? [])
            .Concat(GetXmpArrayValues(xmpMeta, "http://ns.adobe.com/lightroom/1.0/", "hierarchicalSubject").Select(x => x.Replace('|', '/')))
            .ToArray();
    }

    /// <summary>
    /// Read a Windows XP tag (ie: XPTitle, XPComment, XPKeywords). Windows writes these as UCS-2 bytes,
    /// but some tools write them as ASCII/UTF-8 text which would otherwise be decoded as garbled UCS-2.
    /// </summary>
    private static string? GetWindowsXpText(ExifIfd0Directory exifTags, int tagType)
    {
        if (exifTags.GetObject(tagType) is StringValue stringValue)
            return stringValue.ToString(System.Text.Encoding.UTF8).TrimEnd('\0');

        return exifTags.GetDescription(tagType);
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
    /// Returns the first value that is not empty, or null if none are found
    /// </summary>
    private static string? FirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
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
        var rawTagData = TagSplitRegex().Split(tagList.Replace("; ", ";"));
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
            targetKeywordList = TagSplitRegex().Split(targetKeywordString);

        var sourceKeywordList = sourceImage.Keywords.Select(x => x.Trim()).Where(x => x.Length > 0);

        if (sourceKeywordList.Except(targetKeywordList).Any())
            return true;

        if (targetKeywordList.Except(sourceKeywordList).Any())
            return true;

        return false;
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
