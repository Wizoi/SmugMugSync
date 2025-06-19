using System.Text.Json.Serialization;

namespace SmugMugCore.Net.Data20
{
    public class ImageDetail
    {
        [JsonPropertyName("ThumbnailUrl")]
        public string? ThumbnailUrl { get; set; }

        [JsonPropertyName("FileName")]
        public string? FileName { get; set; } 

        [JsonPropertyName("Processing")]
        public bool Processing { get; set; } 

        [JsonPropertyName("UploadKey")]
        public string? UploadKey { get; set; }

        [JsonPropertyName("DateTimeUploaded")]
        public DateTimeOffset DateTimeUploaded { get; set; } 

        [JsonPropertyName("DateTimeOriginal")]
        public DateTimeOffset DateTimeOriginal { get; set; } 

        [JsonPropertyName("Format")]
        public string? Format { get; set; } 

        [JsonPropertyName("OriginalHeight")]
        public int OriginalHeight { get; set; } 

        [JsonPropertyName("OriginalWidth")]
        public int OriginalWidth { get; set; } 

        [JsonPropertyName("OriginalSize")]
        public int OriginalSize { get; set; } 

        [JsonPropertyName("Collectable")]
        public bool Collectable { get; set; } 

        [JsonPropertyName("IsArchive")]
        public bool IsArchive { get; set; } 

        [JsonPropertyName("IsVideo")]
        public bool IsVideo { get; set; } 

        [JsonPropertyName("ComponentFileTypes")]
        public Dictionary<string, List<string>>? ComponentFileTypes { get; set; } 

        [JsonPropertyName("CanEdit")]
        public bool CanEdit { get; set; } 

        [JsonPropertyName("CanBuy")]
        public bool CanBuy { get; set; } 

        [JsonPropertyName("Protected")]
        public bool Protected { get; set; } 

        [JsonPropertyName("EZProject")]
        public bool EZProject { get; set; } 

        [JsonPropertyName("Watermarked")]
        public bool Watermarked { get; set; } 

        [JsonPropertyName("Serial")]
        public int Serial { get; set; } 

        [JsonPropertyName("ArchivedUri")]
        public string? ArchivedUri { get; set; } 

        [JsonPropertyName("ArchivedSize")]
        public int ArchivedSize { get; set; } 

        [JsonPropertyName("ArchivedMD5")]
        public string? ArchivedMD5 { get; set; } 

        [JsonPropertyName("Status")]
        public string? Status { get; set; } 

        [JsonPropertyName("SubStatus")]
        public string? SubStatus { get; set; } 

        [JsonPropertyName("CanShare")]
        public bool CanShare { get; set; } 

        [JsonPropertyName("Comments")]
        public bool Comments { get; set; } 

        [JsonPropertyName("ShowKeywords")]
        public bool ShowKeywords { get; set; } 

        [JsonPropertyName("PreferredDisplayFileExtension")]
        public string? PreferredDisplayFileExtension { get; set; } 

        [JsonPropertyName("Uri")]
        public string? Uri { get; set; } 

        [JsonPropertyName("UriDescription")]
        public string? UriDescription { get; set; }
    
        [JsonPropertyName("Altitude")]
        public double? Altitude { get; set; }

        [JsonPropertyName("Caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("Date")]
        public DateTimeOffset? Date { get; set; }

        [JsonPropertyName("Hidden")]
        public bool? Hidden { get; set; }

        [JsonPropertyName("Keywords")]
        public string? Keywords { get; set; }

        [JsonPropertyName("KeywordsArray")]
        public List<string>? KeywordsArray { get; set; }

        [JsonPropertyName("LastUpdated")]
        public DateTimeOffset? LastUpdated { get; set; }

        [JsonPropertyName("Latitude")]
        public string? Latitude { get; set; }

        [JsonPropertyName("Longitude")]
        public string? Longitude { get; set; }

        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("Watermark")]
        public string? Watermark { get; set; }

        [JsonPropertyName("WebUri")]
        public string? WebUri { get; set; }

        [JsonPropertyName("Uris")]
        public ImageUris? Uris { get; set; }

        [JsonPropertyName("ImageKey")] 
        public string? ImageKey { get; set; }
    }

    public class ImageUris
    {
        [JsonPropertyName("LargestImage")]
        public UriMetadata? LargestImage { get; set; }

        [JsonPropertyName("ImageSizes")]
        public UriMetadata? ImageSizes { get; set; }

        [JsonPropertyName("ImageSizeDetails")]
        public UriMetadata? ImageSizeDetails { get; set; }

        [JsonPropertyName("PointOfInterest")]
        public UriMetadata? PointOfInterest { get; set; }

        [JsonPropertyName("PointOfInterestCrops")]
        public UriMetadata? PointOfInterestCrops { get; set; }

        [JsonPropertyName("Regions")]
        public UriMetadata? Regions { get; set; }

        [JsonPropertyName("ImageAlbum")]
        public UriMetadata? ImageAlbum { get; set; }

        [JsonPropertyName("ImageOwner")]
        public UriMetadata? ImageOwner { get; set; }

        [JsonPropertyName("ImageAlbums")]
        public UriMetadata? ImageAlbums { get; set; }

        [JsonPropertyName("ImageDownload")]
        public UriMetadata? ImageDownload { get; set; }

        [JsonPropertyName("ImageComments")]
        public UriMetadata? ImageComments { get; set; }

        [JsonPropertyName("RotateImage")]
        public UriMetadata? RotateImage { get; set; }

        [JsonPropertyName("ColorImage")]
        public UriMetadata? ColorImage { get; set; }

        [JsonPropertyName("CopyImage")]
        public UriMetadata? CopyImage { get; set; }

        [JsonPropertyName("CropImage")]
        public UriMetadata? CropImage { get; set; }

        [JsonPropertyName("ImageMetadata")]
        public UriMetadata? ImageMetadata { get; set; }

        [JsonPropertyName("ImagePrices")]
        public UriMetadata? ImagePrices { get; set; }

        [JsonPropertyName("ImagePricelistExclusions")]
        public UriMetadata? ImagePricelistExclusions { get; set; }

    }    

}