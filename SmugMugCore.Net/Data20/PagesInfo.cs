using System.Text.Json.Serialization; 

namespace SmugMugCore.Net.Data20
{
    public class PagesInfo
    {
        [JsonPropertyName("Total")]
        public int Total { get; set; }

        [JsonPropertyName("Start")]
        public int Start { get; set; }

        [JsonPropertyName("Count")]
        public int Count { get; set; }

        [JsonPropertyName("RequestedCount")]
        public int RequestedCount { get; set; } 

        [JsonPropertyName("FirstPage")]
        public string? FirstPage { get; set; } 

        [JsonPropertyName("LastPage")]
        public string? LastPage { get; set; } 

        [JsonPropertyName("NextPage")]
        public string? NextPage { get; set; } 

        [JsonPropertyName("PrevPage")]
        public string? PrevPage { get; set; } 

        [JsonPropertyName("Misaligned")]
        public bool Misaligned { get; set; } 
    }
}