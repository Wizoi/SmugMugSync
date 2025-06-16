using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SmugMug.Net.Data; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    // Information from outside the provided sources.
    public class AlbumImageListResponse
    {
        [JsonPropertyName("Response")]
        public AlbumImageListResponseContent Response { get; set; }

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


        public class AlbumImageListResponseContent
        {
            [JsonPropertyName("Uri")]
            public string Uri { get; set; }

            [JsonPropertyName("Locator")]
            public string Locator { get; set; }

            [JsonPropertyName("LocatorType")]
            public string LocatorType { get; set; }

            // If fetching a list of AlbumImages (e.g., from /api/v2/album/{key}!images),
            // you would have a property like this instead of 'Data':
            [JsonPropertyName("AlbumImage")] // Note: SmugMug API sometimes uses singular name for array of objects
            public List<AlbumImageDetail> AlbumImages { get; set; }

            [JsonPropertyName("Pages")]
            public PaginationInfo Pages { get; set; } // The object containing pagination details [1, 2, 4]

            [JsonPropertyName("Timing")]
            public TimingInfo Timing { get; set; } // Information about the request timing [1]
        }
    }
}