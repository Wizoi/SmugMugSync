using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable(), XmlRoot("Image")]
    public class ImageUpload
    {
        /// <summary>
        /// The id for this image
        /// </summary>
        [XmlAttribute("id")]
        public long ImageId;

        /// <summary>
        /// The key for this image
        /// </summary>
        [XmlAttribute("Key")]
        public string ImageKey;

        /// <summary>
        /// URL to the uploaded image
        /// </summary>
        [XmlAttribute("URL")]
        public string URL;

        public ImageUpload()
        {
            ImageKey = String.Empty;
            URL = String.Empty;
        }
    }
}
