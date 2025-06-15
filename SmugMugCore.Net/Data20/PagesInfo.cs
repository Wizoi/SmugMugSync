using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    public class PagesInfo
    {
        [JsonPropertyName("Total")]
        public int Total { get; set; } // The complete number of items available [16, 18]

        [JsonPropertyName("Start")]
        public int Start { get; set; } // The 1-based index of the first item in this response [16, 18]

        [JsonPropertyName("Count")]
        public int Count { get; set; } // The number of items actually returned [16, 18]

        [JsonPropertyName("RequestedCount")]
        public int RequestedCount { get; set; } // The number of items requested [16, 18]

        [JsonPropertyName("FirstPage")]
        public string FirstPage { get; set; } // URI for the first page [16, 18]

        [JsonPropertyName("LastPage")]
        public string LastPage { get; set; } // URI for the last page [16, 18]

        [JsonPropertyName("NextPage")]
        public string NextPage { get; set; } // URI for the next page [16, 18]

        [JsonPropertyName("PrevPage")]
        public string PrevPage { get; set; } // URI for the previous page [18]

        [JsonPropertyName("Misaligned")]
        public bool Misaligned { get; set; } // Indicates if paging is misaligned [18, 21]
    }
}