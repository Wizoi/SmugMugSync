using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmugMug.Net.Data20
{
    public class Album
    {
        [JsonProperty("Uri")]
        public required string Uri { get; set; }

        [JsonProperty("UriDescription")]
        public string UriDescription { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("External")]
        public bool External { get; set; }

        [JsonProperty("ImagesLastUpdated")]
        public DateTime ImagesLastUpdated { get; set; }

        [JsonProperty("LastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("PasswordHint")]
        public string PasswordHint { get; set; }

        [JsonProperty("Privacy")]
        public string Privacy { get; set; }

        [JsonProperty("SmugSearchable")]
        public string SmugSearchable { get; set; }

        [JsonProperty("UrlName")]
        public string UrlName { get; set; }

        [JsonProperty("WebUri")]
        public string WebUri { get; set; }

        [JsonProperty("WorldSearchable")]
        public string WorldSearchable { get; set; }

        [JsonProperty("Node")]
        public string Node { get; set; }

        [JsonProperty("AlbumImages")]
        public string AlbumImages { get; set; }

        [JsonProperty("HighlightImage")]
        public string HighlightImage { get; set; }

        [JsonProperty("User")]
        public string User { get; set; }
    }
}