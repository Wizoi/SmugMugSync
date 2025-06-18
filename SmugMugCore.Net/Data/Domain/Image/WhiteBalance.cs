using System;
using System.Xml.Serialization;

namespace SmugMugCore.Net.Data.Domain.Image
{
    // Indicates the white balance mode set when the image was shot
    public enum WhiteBalance
    {
        [XmlEnum(Name = "0")]
        AutoWhiteBalance = 0,
        
        [XmlEnum(Name = "1")]
        ManualWhiteBalance = 1,
    };
}
