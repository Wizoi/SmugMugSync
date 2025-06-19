using System.Text.Json.Serialization; 

namespace SmugMugCore.Net.Data20
{

    public class AlbumCreateResponse
    {
        [JsonPropertyName("Options")]
        public OptionsInfo? Options { get; set; } 

        [JsonPropertyName("Response")]
        public AlbumDetailCreateResponseData? Response { get; set; }

        [JsonPropertyName("Stat")]
        public string? Status { get; set; }

        [JsonPropertyName("Method")]
        public string? Method { get; set; }

        [JsonPropertyName("Code")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("Message")]
        public string? ErrorMessage { get; set; }
    }

    public class AlbumDetailCreateResponseData
    {
        [JsonPropertyName("Uri")]
        public string? Uri { get; set; } 

        [JsonPropertyName("Locator")]
        public string? Locator { get; set; } 

        [JsonPropertyName("LocatorType")]
        public string? LocatorType { get; set; } 

        [JsonPropertyName("Album")]
        public AlbumDetail? AlbumDetail { get; set; } 

        [JsonPropertyName("UriDescription")]
        public string? UriDescription { get; set; }

        [JsonPropertyName("EndpointType")]
        public string? EndpointType { get; set; } 

        [JsonPropertyName("DocUri")]
        public string? DocUri { get; set; } 
    }
}