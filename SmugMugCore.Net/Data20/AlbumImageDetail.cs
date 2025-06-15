using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SmugMug.Net.Data; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
// Represents an AlbumImage object, which includes all Image fields.
// This class effectively acts as the deserialization target for both Image and AlbumImage properties.
    public class AlbumImage
    {
        [JsonPropertyName("ThumbnailUrl")]
        public string ThumbnailUrl { get; set; } // URL for the thumbnail image

        [JsonPropertyName("FileName")]
        public string FileName { get; set; } // The name of the image file

        [JsonPropertyName("Processing")]
        public bool Processing { get; set; } // Indicates if the image is currently being processed

        [JsonPropertyName("UploadKey")]
        public string UploadKey { get; set; } // Unique key for the upload

        [JsonPropertyName("DateTimeUploaded")]
        public DateTimeOffset DateTimeUploaded { get; set; } // Date and time when the image was uploaded. Using DateTimeOffset is recommended for dates with time zone information.

        [JsonPropertyName("DateTimeOriginal")]
        public DateTimeOffset DateTimeOriginal { get; set; } // Date and time when the original image was taken

        [JsonPropertyName("Format")]
        public string Format { get; set; } // The format of the image (e.g., "JPG")

        [JsonPropertyName("OriginalHeight")]
        public int OriginalHeight { get; set; } // Original height of the image in pixels

        [JsonPropertyName("OriginalWidth")]
        public int OriginalWidth { get; set; } // Original width of the image in pixels

        [JsonPropertyName("OriginalSize")]
        public int OriginalSize { get; set; } // Original size of the image in bytes

        [JsonPropertyName("Collectable")]
        public bool Collectable { get; set; } // Indicates if the image can be collected into other albums [1]

        [JsonPropertyName("IsArchive")]
        public bool IsArchive { get; set; } // Indicates if the image is an archive

        [JsonPropertyName("IsVideo")]
        public bool IsVideo { get; set; } // Indicates if the media is a video rather than a photo

        [JsonPropertyName("ComponentFileTypes")]
        public Dictionary<string, List<string>> ComponentFileTypes { get; set; } // Dictionary to hold file types of components (e.g., "Image": ["jpg"])

        [JsonPropertyName("CanEdit")]
        public bool CanEdit { get; set; } // Indicates if the user has permission to edit the image

        [JsonPropertyName("CanBuy")]
        public bool CanBuy { get; set; } // Indicates if the image can be purchased

        [JsonPropertyName("Protected")]
        public bool Protected { get; set; } // Indicates if the image is protected

        [JsonPropertyName("EZProject")]
        public bool EZProject { get; set; } // Specific SmugMug internal flag

        [JsonPropertyName("Watermarked")]
        public bool Watermarked { get; set; } // Indicates if the image has a watermark [2]

        [JsonPropertyName("Serial")]
        public int Serial { get; set; } // Serial number of the image

        [JsonPropertyName("ArchivedUri")]
        public string ArchivedUri { get; set; } // URI to the archived version of the image

        [JsonPropertyName("ArchivedSize")]
        public int ArchivedSize { get; set; } // Size of the archived image in bytes

        [JsonPropertyName("ArchivedMD5")]
        public string ArchivedMD5 { get; set; } // MD5 hash of the archived image

        [JsonPropertyName("Status")]
        public string Status { get; set; } // Current status of the image (e.g., "Open")

        [JsonPropertyName("SubStatus")]
        public string SubStatus { get; set; } // Sub-status of the image (e.g., "NFS")

        [JsonPropertyName("CanShare")]
        public bool CanShare { get; set; } // Indicates if the image can be shared

        [JsonPropertyName("Comments")]
        public bool Comments { get; set; } // Indicates if comments are enabled for the image

        [JsonPropertyName("ShowKeywords")]
        public bool ShowKeywords { get; set; } // Indicates if keywords should be displayed for the image

        [JsonPropertyName("PreferredDisplayFileExtension")]
        public string PreferredDisplayFileExtension { get; set; } // Preferred file extension for display

        [JsonPropertyName("Uri")]
        public string Uri { get; set; } // The API URI for this specific AlbumImage relationship [1]

        [JsonPropertyName("UriDescription")]
        public string UriDescription { get; set; } // Description of the URI (e.g., "Image from album")
    
        // Important Fields (inherited from Image endpoint) [3]:
        [JsonPropertyName("Altitude")]
        public double? Altitude { get; set; }

        [JsonPropertyName("Caption")]
        public string Caption { get; set; }

        // Date is typically in ISO 8601 format, so DateTimeOffset is a good choice for preserving timezone info.
        [JsonPropertyName("Date")]
        public DateTimeOffset? Date { get; set; }

        [JsonPropertyName("Hidden")]
        public bool? Hidden { get; set; }

        // Keywords are semicolon-separated [3]
        [JsonPropertyName("Keywords")]
        public string Keywords { get; set; }

        // KeywordsArray is a JSON array of keywords [3]
        [JsonPropertyName("KeywordsArray")]
        public List<string> KeywordsArray { get; set; }

        // LastUpdated includes changes to content, properties, or replacement [3]
        [JsonPropertyName("LastUpdated")]
        public DateTimeOffset? LastUpdated { get; set; }

        [JsonPropertyName("Latitude")]
        public string Latitude { get; set; }

        [JsonPropertyName("Longitude")]
        public string Longitude { get; set; }

        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("Watermark")]
        public string Watermark { get; set; }

        // WebUri is the URI to view this image on the SmugMug website [3]
        [JsonPropertyName("WebUri")]
        public string WebUri { get; set; }

        // Important Links (URIs) [2, 4]:
        // These links provide navigation to related resources.
        [JsonPropertyName("Uris")]
        public AlbumImageUris Uris { get; set; }

        // Other Image-specific fields might be present but not explicitly listed in the source,
        // such as `ImageKey`, `AlbumKey` if it's a highlight image within an album response.
        // The Album Search Request and Response sample [5] shows `HighlightAlbumImageUri`
        // within an `Album` object, which points to an `AlbumImage` specific to that context.
        [JsonPropertyName("ImageKey")] // Example, might be present
        public string ImageKey { get; set; }

        [JsonPropertyName("AlbumKey")] // Example, might be present within AlbumImage context
        public string AlbumKey { get; set; }
    }

    // Nested class to hold the URIs related to the AlbumImage
    public class AlbumImageUris
    {
        // Link to the album from this album-image relationship [2]
        [JsonPropertyName("Album")]
        public UriMetadata Album { get; set; }

        // Link to the image from this album-image relationship [2]
        [JsonPropertyName("Image")]
        public UriMetadata Image { get; set; }

        // Link to the album where the image is actually stored (if permitted) [2]
        [JsonPropertyName("ImageAlbum")]
        public UriMetadata ImageAlbum { get; set; }

        // Additional metadata from the image file [4]
        [JsonPropertyName("ImageMetadata")]
        public UriMetadata ImageMetadata { get; set; }

        // Raw media URLs and dimensions for all available sizes of this image [4]
        [JsonPropertyName("ImageSizeDetails")]
        public UriMetadata ImageSizeDetails { get; set; }
    }

}