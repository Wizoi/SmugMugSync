using SixLabors.ImageSharp.Metadata.Profiles.Iptc;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SmugMug.Net.Data;
using SmugMug.Net.Data.Domain;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace SmugMug.Net.Core
{
    /// <summary>
    /// Class to access Image XMP Properties using the Windows Image Components and .Net 3.0
    /// </summary>
    public static partial class ContentMetadataLoader
    {
        public static ImageContent DiscoverMetadata(string filepath)
        {
            ImageContent content;
            if (File.Exists(filepath))
            {
                try
                {
                    SixLabors.ImageSharp.ImageInfo imageInfo = Image.Identify(filepath);
                    content = GetMetadataPropertiesWithImageInfo(imageInfo);
                }
                catch (UnknownImageFormatException)
                {
                    content = GetMetadataPropertiesFromShell(filepath); 
                }
            }
            else
                throw new ApplicationException("File " + filepath + " is not found.");

            content.FileInfo = new FileInfo(filepath);
            return content;
        }

        private static ImageContent GetMetadataPropertiesFromShell(string filepath)
        {
            var content = new ImageContent();
           
            if (ShellObject.IsPlatformSupported)
            {
                ShellProperties? propertyCollection = null;

                // Dispose for the file is unstable for windows as this shell object is not f
                using var file = ShellObject.FromParsingName(filepath);
                propertyCollection = file.Properties;
                if (propertyCollection != null)
                {
                    IShellProperty propertyCaption = propertyCollection.GetProperty("System.Title");
                    if (propertyCaption != null)
                    {
                        content.Caption = (string)propertyCaption.ValueAsObject;
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

            return content;
        }

        /// <summary>
        /// Retrieve the properties we care about
        /// </summary>
        /// <param name="metaData"></param>
        private static ImageContent GetMetadataPropertiesWithImageInfo(SixLabors.ImageSharp.ImageInfo imageInfo)
        {
            var content = new ImageContent();
            ExifProfile? exifData;
            if (imageInfo.FrameMetadataCollection != null && imageInfo.FrameMetadataCollection.Count > 0)
                exifData = imageInfo.FrameMetadataCollection[0].ExifProfile;
            else
                exifData = imageInfo.Metadata.ExifProfile;


            if (exifData != null)
            {
                string dateString = ExtractExifString(exifData, ExifTag.DateTimeOriginal);
                if (dateString.Length > 0)
                {
                    content.DateTaken = DateTime.ParseExact(dateString, "yyyy:MM:dd HH:mm:ss", null);
                }
                else
                    content.DateTaken = DateTime.MinValue;

                content.Caption = ExtractExifString(exifData, ExifTag.XPTitle);
                content.Comment = ExtractExifString(exifData, ExifTag.XPComment);
                content.Keywords = SplitTagString(ExtractExifString(exifData, ExifTag.XPKeywords));

                bool foundOrientation = exifData.TryGetValue(ExifTag.Orientation, out IExifValue<ushort>? exifOrientation);
                if (foundOrientation && exifOrientation != null)
                {
                    content.Orientation = (ContentOrientation)exifOrientation.Value;
                }
            }

            return content;
        }

        private static string[] ExtractIptcStrings(IptcProfile iptcData, IptcTag tag)
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

        private static string ExtractExifString(ExifProfile exifData, ExifTag<string> tag)
        {
            bool foundString = exifData.TryGetValue(tag, out IExifValue<string>? exifString);
            if (foundString && exifString != null)
            {
                return (exifString.Value ?? "").TrimEnd('\0');
            }
            else
                return "";
        }


        /// <summary>
        /// Split a tag into the various elements for Smugmug's Usage
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        private static string[] SplitTagString(string tagList)
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
                        tagData.Add(ContentMetadataLoader.CleanKeywordsForSmugmug(tag));
                    }
                    else
                    {
                        var splitParts = tag.Split('/');
                        foreach (string part in splitParts)
                        {
                            tagData.Add(ContentMetadataLoader.CleanKeywordsForSmugmug(part));
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
        public static bool CompareKeywords(ImageContent sourceImage, string? tagList)
        {
            if (String.IsNullOrWhiteSpace(tagList))
                tagList = string.Empty;

            string[] targetKeywordList;

            string targetKeywordString = tagList.Replace("; ", ";").Trim();
            if (targetKeywordString.Length == 0)
                targetKeywordList = Array.Empty<string>();
            else
                targetKeywordList = TagSplitRegex().Split(targetKeywordString);

            if (sourceImage.Keywords.Except(targetKeywordList).Any())
                return true;

            if (targetKeywordList.Except(sourceImage.Keywords).Any())
                return true;

            return false;
        }

        /// <summary>
        /// SmugmugKeywords need to be uri friendly
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private static string CleanKeywordsForSmugmug(string keyword)
        {
            return keyword.Replace("&", "").Replace("-", "");
        }

        [GeneratedRegex(";")]
        private static partial Regex TagSplitRegex();
    }
}
