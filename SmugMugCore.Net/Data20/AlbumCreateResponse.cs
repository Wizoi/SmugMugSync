using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{

    public class AlbumCreateResponse
    {
        [JsonPropertyName("Options")]
        public OptionsInfo Options { get; set; } // Contains information about endpoint capabilities and parameters [1, 9, 10]

        [JsonPropertyName("Response")]
        public AlbumDetailCreateResponseData Response { get; set; } // The main content of the API response, containing the specific object [1-5]

        [JsonPropertyName("Stat")]
        public string Status { get; set; }

        [JsonPropertyName("Method")]
        public string Method { get; set; }

        [JsonPropertyName("Code")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("Message")]
        public string ErrorMessage { get; set; } 
    }

    /// <summary>
    /// Contains the main data object returned by the API.
    /// </summary>
    public class AlbumDetailCreateResponseData
    {
        [JsonPropertyName("Uri")]
        public string Uri { get; set; } // The URI of the specific object or collection returned [1-5, 13]

        [JsonPropertyName("Locator")]
        public string Locator { get; set; } // The data type of the object, e.g., "Album" or "Node" [1-5, 13]

        [JsonPropertyName("LocatorType")]
        public string LocatorType { get; set; } // Indicates if it's a single "Object" or an array of "Objects" [13]. For creation, it's typically "Object".

        [JsonPropertyName("Album")]
        //public AlbumDetail AlbumDetail { get; set; } // [7]
        public AlbumDetail AlbumDetail { get; set; } // [7]

        [JsonPropertyName("UriDescription")]
        public string UriDescription { get; set; } // A description of the URI [2, 14]

        [JsonPropertyName("EndpointType")]
        public string EndpointType { get; set; } // The type of endpoint, e.g., "AlbumSearch" [1]

        [JsonPropertyName("DocUri")]
        public string DocUri { get; set; } 
    }
}