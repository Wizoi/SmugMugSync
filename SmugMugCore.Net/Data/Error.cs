using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMugCore.Net.Data
{
    /// <summary>
    /// Error from SmugMug
    /// </summary>
    [Serializable(), XmlRoot("err")]
    public class SmugmugError
    {
        /// <summary>
        /// Error Code
        /// </summary>
        [XmlAttribute("code")]
        public int Code;

        /// <summary>
        /// Error Message
        /// </summary>
        [XmlAttribute("msg")]
        public string? Message;
    }
}
