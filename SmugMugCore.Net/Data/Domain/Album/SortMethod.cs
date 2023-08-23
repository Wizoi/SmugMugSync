using System;
using System.Xml.Serialization;

namespace SmugMug.Net.Data.Domain.Album
{
    /// <summary>
    /// The method used for sorting images within this album
    /// </summary>
    public enum SortMethod
    {
        /// <summary>
        /// None (default)
        /// </summary>
        [XmlEnum(Name = "Position")]
        Position,                 
        /// <summary>
        /// By caption
        /// </summary>
        [XmlEnum(Name = "Caption")]
        Caption,                   
        /// <summary>
        /// By filenames
        /// </summary>
        [XmlEnum(Name = "FileName")]
        FileName,                 
        /// <summary>
        /// By date uploaded
        /// </summary>
        [XmlEnum(Name = "Date")]
        Date,
        /// <summary>
        /// By date modified (if available)
        /// </summary>
        [XmlEnum(Name = "DateTime")]
        DateTime,                 
        /// <summary>
         /// By date taken (if available)
        /// </summary>
        [XmlEnum(Name = "DateTimeOriginal")]
        DateTimeOriginal 
    };
}

