using System;
using System.Xml.Serialization;

namespace SmugMug.Net.Data.Domain.Image
{
    // Indicates the direction of sharpness processing applied by the camera when the image was shot
    public enum Sharpness
    {
        [XmlEnum(Name = "0")]
        Normal = 0,
        
        [XmlEnum(Name = "1")]
        Soft = 1,
        
        [XmlEnum(Name = "2")]
        Hard = 2,
    };
}
