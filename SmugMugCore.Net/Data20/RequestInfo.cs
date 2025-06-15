using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    // Represents the "Request" object 
    public class RequestInfo
    {
        [JsonPropertyName("Version")]
        public string Version { get; set; } 

        [JsonPropertyName("Method")]
        public string Method { get; set; } 

        [JsonPropertyName("Uri")]
        public string Uri { get; set; } 
    }
}