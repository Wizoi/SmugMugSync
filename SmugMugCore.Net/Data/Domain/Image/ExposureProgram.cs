using System;
using System.Xml.Serialization;


namespace SmugMug.Net.Data.Domain.Image
{
    // Indicates the program used by the camera to set exposure when the picture is taken
    public enum ExposureProgram
    {
        [XmlEnum(Name = "0")]
        NotDefined = 0,

        [XmlEnum(Name = "1")]
        Manual = 1,
        
        [XmlEnum(Name = "2")]
        NormalProgram = 2,

        [XmlEnum(Name = "3")]
        AperturePriority = 3,

        [XmlEnum(Name = "4")]
        ShutterPriority = 4,

        [XmlEnum(Name = "5")]
        CreativeProgram = 5,

        [XmlEnum(Name = "6")]
        ActionProgram = 6,

        [XmlEnum(Name = "7")]
        PortraitMode = 7,

        [XmlEnum(Name = "8")]
        LandscapeMode = 8,
    };
}
