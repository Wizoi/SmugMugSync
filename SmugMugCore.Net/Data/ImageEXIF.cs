using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SmugMug.Net.Data.Domain.Image;

namespace SmugMug.Net.Data
{
    [Serializable(), XmlRoot("Image")]
    public class ImageExif
    {
        /// <summary>
        /// The id for this image
        /// </summary>
        [XmlAttribute("id")]
        public long ImageId;

        /// <summary>
        /// The key for this image
        /// </summary>
        [XmlAttribute("Key")]
        public string? ImageKey;


        /// <summary>
        /// Indicates the lens aperture set when the image was shot
        /// </summary>
        [XmlAttribute("Aperture")]
        public string? Aperture;

        /// <summary>
        /// Indicates the value of brightness applied by the camera when the image was shot
        /// </summary>
        [XmlAttribute("Brightness")]
        public string? Brightness;

        /// <summary>
        /// Indicates the width of the CCD sensor, given in millimeters (mm)
        /// </summary>
        [XmlAttribute("CCDWidth")]
        public string? CCDWidth;

        /// <summary>
        /// Indicates the colorspace used by the image. Values: 1 - sRGB39321 - Uncalibrated
        /// </summary>
        [XmlAttribute("ColorSpace")]
        public ColorSpace ColorSpace;

        /// <summary>
        /// The compression mode used for a compressed image is indicated in unit bits per pixel
        /// </summary>
        [XmlAttribute("CompressedBitsPerPixel")]
        public string? CompressedBitsPerPixel;

        /// <summary>
        /// Indicates the direction of contrast processing applied by the camera when the image was shot. Values: 0 - Normal1 - Soft2 - Hard
        /// </summary>
        [XmlAttribute("Contrast")]
        public Contrast Contrast;

        /// <summary>
        /// The date and time of image creation/modification
        /// </summary>
        [XmlAttribute("DateTime")]
        public string? DateTimeModified;

        /// <summary>
        /// The date and time when the image was stored as digital data
        /// </summary>
        [XmlAttribute("DateTimeDigitized")]
        public string? DateTimeDigitized;

        /// <summary>
        /// The date and time when the original image data was generated
        /// </summary>
        [XmlAttribute("DateTimeOriginal")]
        public string? DateTimeOriginal;

        /// <summary>
        /// Indicates the digital zoom ratio when the image was shot
        /// </summary>
        [XmlAttribute("DigitalZoomRatio")]
        public string? DigitalZoomRatio;

        /// <summary>
        /// Indicates the exposure bias set when the image was shot
        /// </summary>
        [XmlAttribute("ExposureBiasValue")]
        public string? ExposureBiasValue;

        /// <summary>
        /// Indicates the exposure mode set when the image was shot. Values: 0 - Auto exposure1 - Manual exposure2 - Auto bracket
        /// </summary>
        [XmlAttribute("ExposureMode")]
        public ExposureMode ExposureMode;

        /// <summary>
        /// Indicates the program used by the camera to set exposure when the picture is taken. Values: 0 - Not defined1 - Manual2 - Normal program3 - Aperture priority4 - Shutter priority5 - Creative program6 - Action program7 - Portrait mode8 - Landscape mode
        /// </summary>
        [XmlAttribute("ExposureProgram")]
        public ExposureProgram ExposureProgram;

        /// <summary>
        /// Indicates the exposure time, given in seconds (sec).
        /// </summary>
        [XmlAttribute("ExposureTime")]
        public string? ExposureTime;

        /// <summary>
        /// Indicates the status of flash when the image was shot. Values: 0 - Flash did not fire1 - Flash fired5 - Strobe return light not detected7 - Strobe return light detected9 - Flash fired, compulsory flash mode13 - Flash fired, compulsory flash mode, return light not detected15 - Flash fired, compulsory flash mode, return light detected16 - Flash did not fire, compulsory flash mode24 - Flash did not fire, auto mode25 - Flash fired, auto mode29 - Flash fired, auto mode, return light not detected31 - Flash fired, auto mode, return light detected32 - No flash function65 - Flash fired, red-eye reduction mode69 - Flash fired, red-eye reduction mode, return light not detected71 - Flash fired, red-eye reduction mode, return light detected73 - Flash fired, compulsory flash mode, red-eye reduction mode77 - Flash fired, compulsory flash mode, red-eye reduction mode, return light not detected79 - Flash fired, compulsory flash mode, red-eye reduction mode, return light detected89 - Flash fired, auto mode, red-eye reduction mode93 - Flash fired, auto mode, return light not detected, red-eye reduction mode95 - Flash fired, auto mode, return light detected, red-eye reduction mode
        /// </summary>
        [XmlAttribute("Flash")]
        public Flash Flash;

        /// <summary>
        /// Indicates the actual focal length of the lens, in mm.
        /// </summary>
        [XmlAttribute("FocalLength")]
        public string? FocalLength;

        /// <summary>
        /// Indicates the equivalent focal length assuming a 35mm film camera, in mm.
        /// </summary>
        [XmlAttribute("FocalLengthIn35mmFilm")]
        public string? FocalLengthIn35mmFilm;

        /// <summary>
        /// Indicates the ISO Speed and ISO Latitude of the camera or input device
        /// </summary>
        [XmlAttribute("ISO")]
        public int ISO;

        /// <summary>
        /// Indicates the kind of light source. Values: 0 - Unknown1 - Daylight2 - Fluorescent3 - Tungsten (incandescent light)4 - Flash9 - Fine weather10 - Cloudy weather11 - Shade12 - Daylight fluorescent13 - Day white fluorescent14 - Cool white fluorescent15 - White fluorescent17 - Standard light A18 - Standard light B19 - Standard light C20 - D5521 - D6522 - D7523 - D5024 - ISO studio tungsten255 - Other light source
        /// </summary>
        [XmlAttribute("LightSource")]
        public LightSource LightSource;

        /// <summary>
        /// Indicates the manufacturer of the camera or input device
        /// </summary>
        [XmlAttribute("Make")]
        public string? Make;

        /// <summary>
        /// Indicates the metering mode set when the image was shot. Values: 0 - Unknown1 - Average2 - Center Weighted Average3 - Spot4 - Multi Spot5 - Pattern6 - Partial
        /// </summary>
        [XmlAttribute("Metering")]
        public Metering Metering;

        /// <summary>
        /// Indicates the model name or model number of the camera or input device
        /// </summary>
        [XmlAttribute("Model")]
        public string? Model;

        /// <summary>
        /// Indicates the direction of saturation processing applied by the camera when the image was shot. Values: 0 - Normal1 - Low saturation2 - High saturation
        /// </summary>
        [XmlAttribute("Saturation")]
        public Saturation Saturation;

        /// <summary>
        /// Indicates the image sensor type on the camera or input device. Values: 1 - Not defined2 - One-chip color area sensor3 - Two-chip color area sensor4 - Three-chip color area sensor5 - Color sequential area sensor7 - Trilinear sensor8 - Color sequential linear sensor
        /// </summary>
        [XmlAttribute("SensingMethod")]
        public SensingMethod SensingMethod;

        /// <summary>
        /// Indicates the direction of sharpness processing applied by the camera when the image was shot. Values: 0 - Normal1 - Soft2 - Hard
        /// </summary>
        [XmlAttribute("Sharpness")]
        public Sharpness Sharpness;

        /// <summary>
        /// Indicates the distance to the subject, given in meters
        /// </summary>
        [XmlAttribute("SubjectDistance")]
        public string? SubjectDistance;

        /// <summary>
        /// Indicates the distance to the subject. Values: 0 - Unknown1 - Macro2 - Close view3 - Distant view
        /// </summary>
        [XmlAttribute("SubjectDistanceRange")]
        public SubjectDistanceRange SubjectDistanceRange;

        /// <summary>
        /// Indicates the white balance mode set when the image was shot: 0 - Auto white balance, 1 - Manual white balance
        /// </summary>
        [XmlAttribute("WhiteBalance")]
        public WhiteBalance WhiteBalance;
    }
}
