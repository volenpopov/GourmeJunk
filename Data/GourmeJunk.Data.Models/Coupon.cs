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
        
        public decimal Discount { get; set; }
      
        public decimal? MinimumOrderAmount { get; set; }

        public byte[] Image { get; set; }

        public bool IsActive { get; set; }
    }
}
