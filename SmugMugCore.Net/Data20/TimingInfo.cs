using System.Text.Json.Serialization; 

namespace SmugMugCore.Net.Data20
{
    public class TimingInfo
    {
        [JsonPropertyName("Total")]
        public TimingDetails? Total { get; set; } 
    }
}