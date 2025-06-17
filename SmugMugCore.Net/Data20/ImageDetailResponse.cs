using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SmugMug.Net.Data; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    // Information from outside the provided sources.
    public class ImageDetailResponse
    {
        [JsonPropertyName("Response")]
        public ImageDetailResponseContent Response { get; set; }

        [JsonPropertyName("Stat")]
        public string Status { get; set; }

        [JsonPropertyName("Method")]
        public string Method { get; set; }

        [JsonPropertyName("Code")]
        public int ErrorCode { get; set; } 

        [JsonPropertyName("Message")]
        public string ErrorMessage { get; set; } 

        [JsonPropertyName("Options")]
        public object Options { get; set; }


        public class ImageDetailResponseContent
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
            [JsonPropertyName("Image")]
            public ImageDetail Image { get; set; }
        }
    }
}