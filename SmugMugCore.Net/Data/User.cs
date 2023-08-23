using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable()]
    public class User
    {
        [XmlAttribute("Name")]
        public string? Name;

        [XmlAttribute("NickName")]
        public string? NickName;

        [XmlAttribute("URL")]
        public string? URL;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public User Copy()
        {
            var newObject = (User)MemberwiseClone();
            return newObject;
        }
    }
}
