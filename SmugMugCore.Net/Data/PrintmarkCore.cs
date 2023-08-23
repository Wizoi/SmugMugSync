using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable(), XmlRoot("Printmark")]
    public class PrintmarkCore
    {
        [XmlAttribute("id")]
        public int PrintmarkId;

        [XmlAttribute("Name")]
        public string? Name;

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
