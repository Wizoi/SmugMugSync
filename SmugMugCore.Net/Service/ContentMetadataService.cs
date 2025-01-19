using SixLabors.ImageSharp.Metadata.Profiles.Iptc;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SmugMug.Net.Data;
using SmugMug.Net.Data.Domain;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;
using System.Windows.Media;

namespace SmugMug.Net.Service;

public partial class ContentMetadataService
{
    [GeneratedRegex("[;,]")]
    private partial Regex TagSplitRegex();

    private bool useSixLabors = false;

    public IFileSystem FileSystem = new FileSystem();

    public ContentMetadataService()   
    {}

    public virtual async Task<ImageContent> DiscoverMetadata(string filepath)
    {
        ImageContent content;
        if (this.FileSystem.File.Exists(filepath))
        {
            try
            {
                if (useSixLabors)
                {
                    content = await this.GetMetadataPropertiesWithSixLaborsLibrary(filepath);
                }
                else
                {
                    content= this.GetMetadataPropertiesWithMetadataExtractorLibrary(filepath);
                }
            }
            catch (SixLabors.ImageSharp.UnknownImageFormatException)
            {
                content = this.GetMetadataPropertiesFromShell(filepath); 
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

    private ImageContent GetMetadataPropertiesFromShell(string filepath)
    {
        var content = new ImageContent();
        PopulateMetadataPropertiesFromShell(filepath, content);
        return content;
    }        

    private void PopulateMetadataPropertiesFromShell(string filepath, ImageContent content)
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

                content.Keywords = Array.Empty<string>();
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

    private ImageContent GetMetadataPropertiesWithMetadataExtractorLibrary(string filepath)
    {
        IEnumerable<MetadataExtractor.Directory> directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(filepath);
        var content = new ImageContent();

        var exifDirTags = directories.OfType<MetadataExtractor.Formats.Exif.ExifIfd0Directory>();
        if (exifDirTags.Any())
        {
            var exifTags = exifDirTags.First();
            if (exifTags.HasTagName(ExifIfd0Directory.TagDateTime))
            {
                if (exifTags.TryGetDateTime(ExifIfd0Directory.TagDateTime, out DateTime exifDateTime))
                    content.DateTaken = exifDateTime;
            }
            if (exifTags.HasTagName(ExifIfd0Directory.TagImageDescription))
                content.Title = exifTags.GetDescription(ExifIfd0Directory.TagImageDescription);
            if (exifTags.HasTagName(ExifIfd0Directory.TagWinComment))
                content.Caption = exifTags.GetDescription(ExifIfd0Directory.TagWinComment);
            if (exifTags.HasTagName(ExifIfd0Directory.TagWinKeywords))
                content.Keywords = SplitTagString(exifTags.GetDescription(ExifIfd0Directory.TagWinKeywords) ?? "");

            bool foundOrientation = exifTags.TryGetUInt16(ExifIfd0Directory.TagOrientation, out ushort exifOrientation);
            if (foundOrientation)
            {
                content.Orientation = (ContentOrientation)exifOrientation;
            }
        }

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
    /// Retrieve the properties we care about
    /// </summary>
    /// <param name="metaData"></param>
    private async Task<ImageContent> GetMetadataPropertiesWithSixLaborsLibrary(string filepath)
    {
        SixLabors.ImageSharp.ImageInfo imageInfo = await SixLabors.ImageSharp.Image.IdentifyAsync(filepath);

        var content = new ImageContent();
        ExifProfile? exifData;
        if (imageInfo.FrameMetadataCollection != null && imageInfo.FrameMetadataCollection.Count > 0)
            exifData = imageInfo.FrameMetadataCollection[0].ExifProfile;
        else
            exifData = imageInfo.Metadata.ExifProfile;


        if (exifData != null)
        {
            content.DateTaken = ExtractExifDateTime(ExtractSixLaborsExifString(exifData, ExifTag.DateTimeOriginal));
            content.Title = ExtractSixLaborsExifString(exifData, ExifTag.XPTitle);
            content.Comment = ExtractSixLaborsExifString(exifData, ExifTag.XPComment);
            content.Keywords = SplitTagString(ExtractSixLaborsExifString(exifData, ExifTag.XPKeywords));

            bool foundOrientation = exifData.TryGetValue(ExifTag.Orientation, out IExifValue<ushort>? exifOrientation);
            if (foundOrientation && exifOrientation != null)
            {
                content.Orientation = (ContentOrientation)exifOrientation.Value;
            }
        }

        return content;
    }

    private DateTime ExtractExifDateTime(string exifDateString)
    {
        if (exifDateString.Length > 0)
        {
            return DateTime.ParseExact(exifDateString, "yyyy:MM:dd HH:mm:ss", null);
        }
        else
            return DateTime.MinValue;
    }

    private string[] ExtractIptcStrings(IptcProfile iptcData, IptcTag tag)
    {
        List<string> outputData = new();
        List<IptcValue> iptcStringList = iptcData.GetValues(tag);
        
        foreach (var iptcListValue in iptcStringList)
        {
            outputData.Add(iptcListValue.Value.Trim('\0'));
        }

        // Always have at least an empty string, if there is nothing in IPTC.
        if (outputData.Count == 0)
        {
            outputData.Add("");
        }

        return outputData.ToArray();
    }

    private string ExtractSixLaborsExifString(ExifProfile exifData, ExifTag<string> tag)
    {
        bool foundString = exifData.TryGetValue(tag, out IExifValue<string>? exifString);
        if (foundString && exifString != null)
        {
            return FilterExifString(exifString.Value ?? "");
        }
        else
            return "";
    }

    private string FilterExifString(string exifData)
    {
        return exifData.TrimEnd('\0');
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
    public bool AreKeywordsDifferent(ImageContent sourceImage, string? tagList)
    {
        if (String.IsNullOrWhiteSpace(tagList))
            tagList = string.Empty;

        string[] targetKeywordList;

        string targetKeywordString = tagList.Replace("; ", ";").Replace(", ", ",").Trim();
        if (targetKeywordString.Length == 0)
            targetKeywordList = Array.Empty<string>();
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
