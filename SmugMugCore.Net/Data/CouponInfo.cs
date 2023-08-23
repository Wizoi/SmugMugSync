using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SmugMug.Net.Data.Domain.Coupon;

namespace SmugMug.Net.Data
{
    /// <summary>
    /// Coupon Detailed Information
    /// </summary>
    [Serializable(), XmlRoot("Coupon")]
    public class CouponInfo: CouponCore
    {
        /// <summary>
        /// The total amount (in dollars) discounted using this coupon.
        /// </summary>
        [XmlAttribute("Amount")]
        public float Amount;

        /// <summary>
        /// The code for this coupon
        /// </summary>
        [XmlAttribute("Code")]
        public string? Code;

        /// <summary>
        /// The description for this coupon
        /// </summary>
        [XmlAttribute("Description")]
        public string? Description;

        /// <summary>
        /// The discount (in dollars or percent) for this coupon.
        /// </summary>
        [XmlAttribute("Discount")]
        public float Discount;

        /// <summary>
        /// Allow this coupon to cover international shipping costs.
        /// </summary>
        [XmlAttribute("International")]
        public bool InternationalShippingCovered;

        /// <summary>
        /// The maximum discount (in dollars) allowed for this coupon.
        /// (Percent coupons only) 
        /// </summary>
        [XmlAttribute("MaxDiscount")]
        public float MaxDiscount;

        /// <summary>
        /// The maximum number of uses allowed for this coupon.
        /// (Amount, Cost, Percent & Shipping coupons only) 
        /// </summary>
        [XmlAttribute("MaxUses")]
        public float MaxUses;

        /// <summary>
        /// The minimum purchase (in dollars) required for this coupon.
        /// (Amount, Percent & Shipping coupons only) 
        /// </summary>
        [XmlAttribute("MinPurchase")]
        public float MinPurchase;

        /// <summary>
        /// Albums restricted from using this coupon
        /// </summary>
        [XmlArray("Restrictions")]
        [XmlArrayItem("Album")]
        public AlbumDetail[]? Restrictions;

        /// <summary>
        /// Allow this coupon to cover shipping costs.
        /// (Amount, Credit & Percent coupons only) 
        /// </summary>
        [XmlAttribute("Shipping")]
        public bool ShippingCovered;

        /// <summary>
        /// The status for this coupon
        /// </summary>
        [XmlAttribute("Status")]
        public CouponStatus Status;

        /// <summary>
        /// The title for this coupon
        /// </summary>
        [XmlAttribute("Title")]
        public string? Title;

        /// <summary>
        /// The type of coupon
        /// </summary>
        [XmlAttribute("Type")]
        public CouponType Type;

        /// <summary>
        /// The number of times this coupon has been used
        /// </summary>
        [XmlAttribute("Uses")]
        public int UsageCount;

        /// <summary>
        /// The date this coupon is valid from
        /// </summary>
        [XmlAttribute("ValidFrom")]
        public string? ValidFromDate;

        /// <summary>
        /// The date this coupon is valid to
        /// </summary>
        [XmlAttribute("ValidTo")]
        public string? ValidToDate;

        /// <summary>
        /// Copy this object
        /// </summary>
        /// <returns></returns>
        public override object Copy()
        {
            var newObject = (CouponInfo)MemberwiseClone();
            if (this.Restrictions != null)
            {
                // Copy the list of restrictions
                var newAlbums = this.Restrictions.Select(x => x.Copy());
                newObject.Restrictions = (AlbumDetail[])newAlbums.ToArray();
            }
            return newObject;
        }
    }
}
