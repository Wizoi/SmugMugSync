using System.Text.Json.Serialization;

namespace SmugMugCore.Net.Data20
{
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