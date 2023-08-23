using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable()]
    public class Template
    {
        [XmlAttribute("id")]
        public int TemplateId;

        [XmlAttribute("Name")]
        public string? Name;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public Template Copy()
        {
            var newObject = (Template)MemberwiseClone();
            return newObject;
        }
    }
}
