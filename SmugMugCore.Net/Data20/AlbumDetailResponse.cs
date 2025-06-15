using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{

    // Represents the overall structure of the API response
    public class AlbumDetailResponse
    {
        [JsonPropertyName("Request")]
        public RequestInfo Request { get; set; } // Includes details about the request [1]

        [JsonPropertyName("Options")]
        public OptionsInfo Options { get; set; } // Includes method/parameter options, media types, etc. [1, 5]

        [JsonPropertyName("Response")]
        public AlbumDetailResponseContent Response { get; set; } // The main response payload [1, 3, 6]

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
    public class AlbumDetailResponseContent
    {
        [JsonPropertyName("Uri")]
        public string Uri { get; set; } // The URI of the requested resource [1, 8]

        [JsonPropertyName("Locator")]
        public string Locator { get; set; } // The data type of the object(s) in the response (e.g., "Album", "User") [1, 8]

        [JsonPropertyName("LocatorType")]
        public string LocatorType { get; set; } // Indicates if the response contains a single "Object" or an array of "Objects" [1, 8]

        // This property will hold the actual data based on the endpoint.
        // In the provided sample [1], it's an array of Albums.
        // You would need to define a property for the specific data type expected.
        // For the Album Search sample:
        [JsonPropertyName("Album")] // Matches the key name in the JSON sample [3]
        public AlbumDetail AlbumDetail { get; set; } // This would be List<User>, List<Node>, etc. for other endpoints

        [JsonPropertyName("UriDescription")]
        public string UriDescription { get; set; } // Human-readable description of the URI [1, 6]

        [JsonPropertyName("EndpointType")]
        public string EndpointType { get; set; } // Type of endpoint (e.g., "AlbumSearch", "User") [1, 6]

        [JsonPropertyName("Pages")]
        public PaginationInfo Pages { get; set; } // The object containing pagination details [1, 2, 4]

        [JsonPropertyName("Timing")]
        public TimingInfo Timing { get; set; } // Information about the request timing [1]

        // Depending on the endpoint, other top-level response properties might exist.
        // For example, a single object response might have a "User" property directly here instead of "Album"
        // and LocatorType would be "Object".
    }
}