using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    [Serializable()]
    public class ShareGroup
    {
        /// <summary>
        /// The id for this sharegroup
        /// </summary>
        [XmlAttribute("id")]
        public int ShareGroupId;

        /// <summary>
        /// The tag (public id) for this sharegroup
        /// </summary>
        [XmlAttribute("Tag")]
        public string? Tag;

        /// <summary>
        /// Allow access to password protected albums from this sharegroup without the password
        /// </summary>
        [XmlAttribute("AccessPassworded")]
        public bool AccessPassworded;

        /// <summary>
        /// The number of albums in this sharegroup
        /// </summary>
        [XmlAttribute("AlbumCount")]
        public int AlbumCount;

        /// <summary>
        /// Albums within this ShareGroup
        /// </summary>
        [XmlArray("Albums")]
        [XmlArrayItem("Album")]
        public AlbumDetail[]? Albums;

        /// <summary>
        /// The description for this sharegroup
        /// </summary>
        [XmlAttribute("Description")]
        public string? Description;

        /// <summary>
        /// The Name for this sharegroup
        /// </summary>
        [XmlAttribute("Name")]
        public string? Name;

        /// <summary>
        /// The Password for this sharegroup
        /// </summary>
        [XmlAttribute("Password")]
        public string? Password;

        /// <summary>
        /// The Password hint for this sharegroup
        /// </summary>
        [XmlAttribute("PasswordHint")]
        public string? PasswordHint;

        /// <summary>
        /// Indicates whether this sharegroup is password protected.
        /// </summary>
        [XmlAttribute("Passworded")]
        public bool PasswordEnabled;

        /// <summary>
        /// The URL for this sharegroup
        /// </summary>
        [XmlAttribute("URL")]
        public string? URL;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public ShareGroup Copy()
        {
            var newObject = (ShareGroup)MemberwiseClone();
            if (this.Albums != null)
            {
                // Copy the list of restrictions
                var newAlbums = this.Albums.Select(x => x.Copy());
                newObject.Albums = (AlbumDetail[])newAlbums.ToArray();
            }
            return newObject;
        }
    }
}
