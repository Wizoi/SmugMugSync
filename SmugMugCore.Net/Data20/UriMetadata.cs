using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    /// <summary>
    /// Represents linked URIs with full metadata, typical for unfiltered responses.
    /// If _shorturis is used, this structure would simplify (e.g., to Dictionary<string, string>). [19, 20]
    /// </summary>
    public class UriMetadata
    {
        [JsonPropertyName("Uri")]
        public string Uri { get; set; } // The API URI [14]

        [JsonPropertyName("Locator")]
        public string Locator { get; set; } // Data type of the linked object [14]

        [JsonPropertyName("LocatorType")]
        public string LocatorType { get; set; } // "Object" or "Objects" [14]

        [JsonPropertyName("UriDescription")]
        public string UriDescription { get; set; } // Description of the URI [14]

        [JsonPropertyName("EndpointType")]
        public string EndpointType { get; set; } // Type of endpoint [14]
    }
}