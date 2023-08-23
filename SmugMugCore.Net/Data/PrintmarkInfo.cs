using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable(), XmlRoot("Printmark")]
    public class PrintmarkInfo
    {
        [XmlAttribute("id")]
        public int PrintmarkId;

        [XmlAttribute("Name")]
        public string? Name;
        
        /// <summary>
        /// The opacity of this printmark on the target image
        /// </summary>
        [XmlAttribute("Dissolve")]
        public int Dissolve;

        /// <summary>
        /// The location of this printmark on the target image
        /// </summary>
        [XmlAttribute("Location")]
        public SmugMug.Net.Data.Domain.Printmark.Location Location;

        /// <summary>
        /// Target Image printmark is applied to
        /// </summary>
        [XmlElement("Image")]
        public ImageDetail? Image;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public object Copy()
        {
            var newObject = (PrintmarkInfo)MemberwiseClone();
            if (this.Image != null)
                newObject.Image = (ImageDetail)this.Image.Copy();
            return newObject;
        }
    }
}
