using System;
using System.Xml.Serialization;


namespace SmugMug.Net.Data.Domain.Image
{
    // Indicates the exposure mode set when the image was shot
    public enum ExposureMode
    {
        [XmlEnum(Name = "0")]
        AutoExposure = 0,

        [XmlEnum(Name = "1")]
        ManualExposure = 1,
        
        [XmlEnum(Name = "2")]
        AutoBracket = 2,
    };
}
