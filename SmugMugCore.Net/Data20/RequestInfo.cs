using System.Text.Json.Serialization; 

namespace SmugMugCore.Net.Data20
{
    public class RequestInfo
    {
        [JsonPropertyName("Version")]
        public string? Version { get; set; } 

        [JsonPropertyName("Method")]
        public string? Method { get; set; } 

        [JsonPropertyName("Uri")]
        public string? Uri { get; set; } 
    }
}