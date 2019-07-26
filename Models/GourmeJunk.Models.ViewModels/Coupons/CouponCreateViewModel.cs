using GourmeJunk.Data.Models;
using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.Coupons
{
    public class CouponCreateViewModel
    {                
        public string Name { get; set; }

        public string CouponType { get; set; }
        
        public decimal Discount { get; set; }
        
        [Display(Name = ModelConstants.Coupon.MIN_ORDER_AMOUNT_DISPLAY)]
        public decimal? MinimumOrderAmount { get; set; }

        [Display(Name = ModelConstants.Coupon.IS_ACTIVE_DISPLAY)]
        public bool IsActive { get; set; }

        public string StatusMessage { get; set; }
    }
}
