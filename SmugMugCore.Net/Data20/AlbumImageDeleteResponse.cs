using System.Text.Json.Serialization; 

namespace SmugMugCore.Net.Data20
{

    public class AlbumImageDeleteResponse
    {
        [JsonPropertyName("Options")]
        public OptionsInfo? Options { get; set; } 

        [JsonPropertyName("Response")]
        public AlbumImageDeleteResponseContent? Response { get; set; } 

        [JsonPropertyName("Stat")]
        public string? Status { get; set; }

        [JsonPropertyName("Method")]
        public string? Method { get; set; }

        [JsonPropertyName("Code")]
        public int ErrorCode { get; set; } 

        [JsonPropertyName("Message")]
        public string? ErrorMessage { get; set; } 
    }

    public class AlbumImageDeleteResponseContent
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