using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Image Highlight for an Album
    /// </summary>
    [Serializable()]
    public class Highlight
    {
        /// <summary>
        /// ID of Highlight Image
        /// </summary>
        [XmlAttribute("id")]
        public long HighlightImageId;

        /// <summary>
        /// Key of Highlight Image
        /// </summary>
        [XmlAttribute("Key")]
        public string? HighlightImageKey;

        /// <summary>
        /// Type of Highlight Image
        /// </summary>
        [XmlAttribute("Type")]
        public string? HighlightImageType;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public Highlight Copy()
        {
            var newObject = (Highlight)MemberwiseClone();
            return newObject;
        }
    }
}
