using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Core field for a Coupon
    /// </summary>
    [Serializable(), XmlRoot("Coupon")]
    public class CouponCore
    {
        /// <summary>
        /// The id for this coupon
        /// </summary>
        [XmlAttribute("id")]
        public int CouponId;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public virtual object Copy()
        {
            var newObject = MemberwiseClone();
            return newObject;
        }

    }
}
