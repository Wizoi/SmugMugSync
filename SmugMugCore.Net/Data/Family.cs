using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Family
    /// 
    /// </summary>
    [Serializable()]
    public class Family
    {
        /// <summary>
        /// The Name for this user
        /// </summary>
        [XmlAttribute("Name")]
        public string? Name;

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
        public Family Copy()
        {
            var newObject = (Family)MemberwiseClone();
            return newObject;
        }
    }
}
