using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{

    /// <summary>
    /// Base Album details used in referential objects
    /// </summary>
    [Serializable(), XmlRoot("Album")]
    public class AlbumCore
    {
        public AlbumCore() 
        {
            AlbumKey = string.Empty;
            Url = string.Empty;
        }

        /// <summary>
        /// ID for this Album
        /// </summary>
        [XmlAttribute("id")]
        public int AlbumId;

        /// <summary>
        /// Key for this Album
        /// </summary>
        [XmlAttribute("Key")]
        public string AlbumKey;
        
        /// <summary>
        /// URL for this Album
        /// </summary>
        [XmlAttribute("URL")]
        public string Url;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public virtual object Copy()
        {
            var newObject = MemberwiseClone();
            return newObject;
        }

    }
}
