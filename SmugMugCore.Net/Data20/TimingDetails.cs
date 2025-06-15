using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    // Represents the details within the "Total" timing object [1]
    public class TimingDetails
    {
        [JsonPropertyName("time")]
        public decimal Time { get; set; } 

        [JsonPropertyName("cycles")]
        public int Cycles { get; set; } 

        [JsonPropertyName("objects")]
        public int Objects { get; set; } 
    }
}