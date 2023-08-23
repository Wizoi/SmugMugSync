using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Comments for a specific Album
    /// </summary>
    [Serializable(), XmlRoot("Album")]
    public class AlbumComments
    {
        /// <summary>
        /// The id for this album
        /// </summary>
        [XmlAttribute("id")]
        public int AlbumId;

        /// <summary>
        /// The key for this album
        /// </summary>
        [XmlAttribute("Key")]
        public string? AlbumKey;

        /// <summary>
        /// Comments for the Image
        /// </summary>
        [XmlElement("Comments")]
        public Comment[]? Comments;
    }
}
