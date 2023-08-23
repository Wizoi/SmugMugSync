using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Fan
    /// </summary>
    [Serializable()]
    public class Fan
    {
        /// <summary>
        /// The Name for this user
        /// </summary>
        [XmlAttribute("Name")]
        public int Name;

        /// <summary>
        /// The NickName for this user
        /// </summary>
        [XmlAttribute("NickName")]
        public string? NickName;

        /// <summary>
        /// The homepage URL for this user
        /// </summary>
        [XmlAttribute("URL")]
        public string? URL;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public Fan Copy()
        {
            var newObject = (Fan)MemberwiseClone();
            return newObject;
        }

    }
}
