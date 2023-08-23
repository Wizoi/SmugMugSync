using System;
using System.Xml.Serialization;

namespace SmugMug.Net.Data.Domain.Printmark
{
    //The location of the printmark on the target image. 
    public enum Location
    {
        [XmlEnum(Name = "Bottom")]
        Bottom,

        [XmlEnum(Name = "BottomLeft")]
        BottomLeft,

        [XmlEnum(Name = "BottomRight")]
        BottomRight,

        [XmlEnum(Name = "Top")]
        Top,

        [XmlEnum(Name = "TopLeft")]
        TopLeft,

        [XmlEnum(Name = "TopRight")]
        TopRight
    };



}
