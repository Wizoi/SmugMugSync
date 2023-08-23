using System;
using System.Xml.Serialization;

namespace SmugMug.Net.Data.Domain.Watermark
{
    //The location of the watermark on the target image. 
    public enum Pinned
    {
        [XmlEnum(Name = "Bottom")]
        Bottom,
        
        [XmlEnum(Name = "BottomLeft")]
        BottomLeft,
        
        [XmlEnum(Name = "BottomRight")]
        BottomRight,

        [XmlEnum(Name = "Center")]
        Center,
        
        [XmlEnum(Name = "Left")]
        Left,
        
        [XmlEnum(Name = "Right")]
        Right,
        
        [XmlEnum(Name = "Tile")]
        Tile,
        
        [XmlEnum(Name = "Top")]
        Top,
        
        [XmlEnum(Name = "TopLeft")]
        TopLeft,
        
        [XmlEnum(Name = "TopRight")]
        TopRight
    };



}
