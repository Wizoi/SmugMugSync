using System.Text.Json.Serialization;

namespace SmugMugCore.Net.Data20
{
    public class AlbumImageDetail
    {
        /// <summary>
        /// Field used to determine if this image is in a deleted state during processing
        /// </summary>
        public bool IsDeleted { get; set; }

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
        public AlbumImageUris? Uris { get; set; }

        [JsonPropertyName("ImageKey")]
        public string? ImageKey { get; set; }

        [JsonPropertyName("AlbumKey")]
        public string? AlbumKey { get; set; }
    }

    public class AlbumImageUris
    {
        [JsonPropertyName("Album")]
        public UriMetadata? Album { get; set; }

        [JsonPropertyName("Image")]
        public UriMetadata? Image { get; set; }

        [JsonPropertyName("ImageAlbum")]
        public UriMetadata? ImageAlbum { get; set; }

        [JsonPropertyName("ImageMetadata")]
        public UriMetadata? ImageMetadata { get; set; }

        [JsonPropertyName("ImageSizeDetails")]
        public UriMetadata? ImageSizeDetails { get; set; }
    }
}