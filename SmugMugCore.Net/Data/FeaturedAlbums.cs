
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Featured Albums for a site
    /// </summary>
    [Serializable(), XmlRoot("Featured")]
    public class FeaturedAlbums
    {
        /// <summary>
        /// Albums that are featured
        /// </summary>
        [XmlArray("Albums")]
        [XmlArrayItem("Album")]
        public AlbumDetail[]? Albums;
    }
}
