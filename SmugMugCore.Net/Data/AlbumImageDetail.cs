using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Images for a given album with count
    /// </summary>
    [Serializable(), XmlRoot("Album")]
    public class AlbumImageDetail: AlbumCore
    {
        /// <summary>
        /// The number of images in this album
        /// </summary>
        [XmlAttribute("ImageCount")]
        public int ImageCount;

        /// <summary>
        /// Images in Album with full information
        /// </summary>
        [XmlElement("Images")]
        public ImageInfo[]? Images;
    }
}
