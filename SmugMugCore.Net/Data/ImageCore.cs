using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable(), XmlRoot("Image")]
    public class ImageCore
    {
        /// <summary>
        /// The id for this image
        /// </summary>
        [XmlAttribute("id")]
        public int ImageId;

        /// <summary>
        /// The key for this image
        /// </summary>
        [XmlAttribute("Key")]
        public string? ImageKey;

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
