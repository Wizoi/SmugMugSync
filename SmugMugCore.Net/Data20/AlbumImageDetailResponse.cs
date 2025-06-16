using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SmugMug.Net.Data; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    // Information from outside the provided sources.
    public class AlbumImageDetailResponse
    {
        [JsonPropertyName("Response")]
        public AlbumImageDetailResponseContent Response { get; set; }

        [JsonPropertyName("Stat")]
        public string Status { get; set; }

        [JsonPropertyName("Method")]
        public string Method { get; set; }

        [JsonPropertyName("Code")]
        public int ErrorCode { get; set; } 

        [JsonPropertyName("Message")]
        public string ErrorMessage { get; set; } 

        // Optional: Request and Options sections could be added if needed for completeness
        [JsonPropertyName("Request")]
        public object Request { get; set; }

        [JsonPropertyName("Options")]
        public object Options { get; set; }


        public class AlbumImageDetailResponseContent
        {
            [JsonPropertyName("Uri")]
            public string Uri { get; set; }

            [JsonPropertyName("Locator")]
            public string Locator { get; set; }

            [JsonPropertyName("LocatorType")]
            public string LocatorType { get; set; }

            // This property will hold the actual AlbumImage object(s).
            // The name might vary depending on whether it's a single object or an array.
            // For a single AlbumImage (e.g., from HighlightAlbumImageUri), it's often directly named "AlbumImage".
            // For a list of images in an album (e.g., from AlbumImages), it would be an array named "AlbumImages".
            // Assuming a single object response for this class.
            [JsonPropertyName("AlbumImage")]
            public AlbumImageDetail AlbumImage { get; set; }

            // If fetching a list of AlbumImages (e.g., from /api/v2/album/{key}!images),
            // you would have a property like this instead of 'Data':
            // [JsonPropertyName("AlbumImage")] // Note: SmugMug API sometimes uses singular name for array of objects
            // public List<T> AlbumImages { get; set; }

            // Optional: Pages and Timing sections could be added for paginated responses or performance metrics
            [JsonPropertyName("Pages")]
            public object Pages { get; set; }

            [JsonPropertyName("Timing")]
            public object Timing { get; set; }
        }
    }
}