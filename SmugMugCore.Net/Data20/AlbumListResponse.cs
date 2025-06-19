using System.Text.Json.Serialization;

namespace SmugMugCore.Net.Data20
{

    public class AlbumListResponse
    {
        [JsonPropertyName("Request")]
        public RequestInfo? Request { get; set; } 

        [JsonPropertyName("Options")]
        public OptionsInfo? Options { get; set; } 

        [JsonPropertyName("Response")]
        public AlbumListResponseContent? Response { get; set; } 

        [JsonPropertyName("Stat")]
        public string? Status { get; set; }

        [JsonPropertyName("Method")]
        public string? Method { get; set; }

        [JsonPropertyName("Code")]
        public int ErrorCode { get; set; } 

        [JsonPropertyName("Message")]
        public string? ErrorMessage { get; set; } 
    }

    public class AlbumListResponseContent
    {
        [JsonPropertyName("Uri")]
        public string? Uri { get; set; } 

        [JsonPropertyName("Locator")]
        public string? Locator { get; set; } 

        [JsonPropertyName("LocatorType")]
        public string? LocatorType { get; set; } 

        [JsonPropertyName("Album")] 
        public List<AlbumDetail>? AlbumDetail { get; set; } 

        [JsonPropertyName("UriDescription")]
        public string? UriDescription { get; set; } 

        [JsonPropertyName("EndpointType")]
        public string? EndpointType { get; set; } 

        [JsonPropertyName("Pages")]
        public PaginationInfo? Pages { get; set; } 

        [JsonPropertyName("Timing")]
        public TimingInfo? Timing { get; set; } 
    }
}