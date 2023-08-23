using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Community
    /// </summary>
    [Serializable()]
    public class Community
    {
        /// <summary>
        /// ID for a specific community
        /// </summary>
        [XmlAttribute("id")]
        public int CommunityId;

        /// <summary>
        /// Name of comunity
        /// </summary>
        [XmlAttribute("Name")]
        public string? Name;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public Community Copy()
        {
            var newObject = (Community)MemberwiseClone();
            return newObject;
        }

    }
}
