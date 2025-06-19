using System.Text.Json.Serialization; 

namespace SmugMugCore.Net.Data20
{
    public class UriMetadata
    {
        [JsonPropertyName("Uri")]
        public string? Uri { get; set; } 

        [JsonPropertyName("Locator")]
        public string? Locator { get; set; } 

        [JsonPropertyName("LocatorType")]
        public string? LocatorType { get; set; } 

        [JsonPropertyName("UriDescription")]
        public string? UriDescription { get; set; } 

        [JsonPropertyName("EndpointType")]
        public string? EndpointType { get; set; } 
    }
}