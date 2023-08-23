using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable()]
    public class Theme
    {
        [XmlAttribute("id")]
        public int ThemeId;

        [XmlAttribute("Name")]
        public string? Name;

        [XmlAttribute("Type")]
        public SmugMug.Net.Data.Domain.Theme.ThemeType Type;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public Theme Copy()
        {
            var newObject = (Theme)MemberwiseClone();
            return newObject;
        }
    }
}
