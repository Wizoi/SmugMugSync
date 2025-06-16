using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{

    // Represents the overall structure of the API response
    public class AlbumImageDeleteResponse
    {
        [JsonPropertyName("Options")]
        public OptionsInfo Options { get; set; } // Includes method/parameter options, media types, etc. [1, 5]

        [JsonPropertyName("Response")]
        public AlbumImageDeleteResponseContent Response { get; set; } // The main response payload [1, 3, 6]

        [JsonPropertyName("Stat")]
        public string Status { get; set; }

        [JsonPropertyName("Method")]
        public string Method { get; set; }

        [JsonPropertyName("Code")]
        public int ErrorCode { get; set; } 

        [JsonPropertyName("Message")]
        public string ErrorMessage { get; set; } 
    }

    // Represents the content within the "Response" key
    public class AlbumImageDeleteResponseContent
    {
        [JsonPropertyName("Uri")]
        public string Uri { get; set; } // The URI of the requested resource [1, 8]

        [JsonPropertyName("Locator")]
        public string Locator { get; set; } // The data type of the object(s) in the response (e.g., "Album", "User") [1, 8]

        [JsonPropertyName("LocatorType")]
        public string LocatorType { get; set; } // Indicates if the response contains a single "Object" or an array of "Objects" [1, 8]

        [JsonPropertyName("UriDescription")]
        public string UriDescription { get; set; } // Human-readable description of the URI [1, 6]

        [JsonPropertyName("EndpointType")]
        public string EndpointType { get; set; } // Type of endpoint (e.g., "AlbumSearch", "User") [1, 6]
    }
}