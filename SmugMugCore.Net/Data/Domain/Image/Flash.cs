using System;
using System.Xml.Serialization;

namespace SmugMug.Net.Data.Domain.Image
{
    // Indicates the status of flash when the image was shot
    public enum Flash
    {
        [XmlEnum(Name = "0")]
        FlashDidNotFire = 0,
        [XmlEnum(Name = "1")]
        FlashFired = 1,
        [XmlEnum(Name = "5")]
        StrobeReturnLightNotDetected = 5,
        [XmlEnum(Name = "7")]
        StrobeReturnLightDetected = 7,
        [XmlEnum(Name = "9")]
        FlashFiredCompulsoryFlashMode = 9,
        [XmlEnum(Name = "13")]
        FlashFiredCompulsoryFlashModeReturnLightNotDetected = 13,
        [XmlEnum(Name = "15")]
        FlashFiredCompulsoryFlashModeReturnLightDetected = 15,
        [XmlEnum(Name = "16")]
        FlashDidNotFireCompulsoryFlashMode = 16,
        [XmlEnum(Name = "24")]
        FlashDidNotFireAutoMode = 24,
        [XmlEnum(Name = "25")]
        FlashFiredAutoMode = 25,
        [XmlEnum(Name = "29")]
        FlashFiredAutoModeReturnLightNotDetected = 29,
        [XmlEnum(Name = "31")]
        FlashFiredAutoModeReturnLightDetected = 31,
        [XmlEnum(Name = "32")]
        NoFlashFunction = 32,
        [XmlEnum(Name = "65")]
        FlashFiredRedEyeReductionMode = 65,
        [XmlEnum(Name = "69")]
        FlashFiredRedEyeReductionModeReturnLightNotDetected = 69,
        [XmlEnum(Name = "71")]
        FlashFiredRedEyeReductionModeReturnLightDetected = 71,
        [XmlEnum(Name = "73")]
        FlashFiredCompulsoryFlashModeRedEyeReductionMode = 73,
        [XmlEnum(Name = "77")]
        FlashFiredCompulsoryFlashModeRedEyeReductionModeReturnLightNotDetected = 77,
        [XmlEnum(Name = "79")]
        FlashFiredCompulsoryFlashModeRedEyeReductionModeReturnLightDetected = 79,
        [XmlEnum(Name = "89")]
        FlashFiredAutoModeRedEyeReductionMode = 89,
        [XmlEnum(Name = "93")]
        FlashFiredAutoModeReturnLightNotDetectedRedEyeReductionMode = 93,
        [XmlEnum(Name = "95")]
        FlashFiredAutoModeReturnLightDetectedRedEyeReductionMode = 95,
    };
}
