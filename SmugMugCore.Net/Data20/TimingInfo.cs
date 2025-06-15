using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace SmugMug.Net.Data20
{
    // Represents the "Timing" object [1]
    public class TimingInfo
    {
        [JsonPropertyName("Total")]
        public TimingDetails Total { get; set; } 
    }
}