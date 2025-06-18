using System.Text.Json.Serialization; 

namespace SmugMugCore.Net.Data20
{
    public class ParameterInfo
    {
        [JsonPropertyName("Name")]
        public string? Name { get; set; } 

        [JsonPropertyName("Required")]
        public bool Required { get; set; }

        [JsonPropertyName("ReadOnly")]
        public bool ReadOnly { get; set; }

        [JsonPropertyName("Default")]
        public object? Default { get; set; } 

        [JsonPropertyName("Description")]
        public string? Description { get; set; } 

        [JsonPropertyName("Type")]
        public string? Type { get; set; } 

        [JsonPropertyName("MIN_CHARS")]
        public object? MIN_CHARS { get; set; } 

        [JsonPropertyName("MAX_CHARS")]
        public object? MAX_CHARS { get; set; } 

        [JsonPropertyName("OPTIONS")]
        public List<string>? OPTIONS { get; set; } 

        [JsonPropertyName("MIN_COUNT")]
        public int? MIN_COUNT { get; set; } 

        [JsonPropertyName("MAX_COUNT")]
        public int? MAX_COUNT { get; set; } 

        [JsonPropertyName("Locator")]
        public List<string>? Locator { get; set; } 

        [JsonPropertyName("Value")]
        public object? Value { get; set; } 

        [JsonPropertyName("Label")]
        public string? Label { get; set; } 
    }
}