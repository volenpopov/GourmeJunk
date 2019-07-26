using GourmeJunk.Data.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class Coupon : BaseDeletableModel<string>
    {
        [Required]
        [MaxLength(DataModelConstants.COUPON_NAME_MAX_LENGTH)]
        public string Name { get; set; }
        
        public CouponType CouponType { get; set; }
        
        [Range(typeof(decimal), DataModelConstants.MIN_PRICE, DataModelConstants.MAX_PRICE)]
        public decimal Discount { get; set; }

        [Range(typeof(decimal), DataModelConstants.MIN_PRICE, DataModelConstants.MAX_PRICE)]
        public decimal? MinimumOrderAmount { get; set; }

        public byte[] Image { get; set; }

        public bool IsActive { get; set; }
    }
}
