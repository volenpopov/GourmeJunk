using GourmeJunk.Data.Models;
using GourmeJunk.Models.Common;
using GourmeJunk.Services.Mapping;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.Coupons
{
    public class CouponViewModel : IMapFrom<Coupon>
    {
        public string Id { get; set; }

        [Display(Name = ModelConstants.Coupon.NAME_DISPLAY)]
        public string Name { get; set; }

        public CouponType CouponType { get; set;}

        public decimal Discount { get; set; }

        [Display(Name = ModelConstants.Coupon.MIN_ORDER_AMOUNT_DISPLAY)]
        public decimal MinimumOrderAmount { get; set; }

        [Display(Name = ModelConstants.Coupon.IS_ACTIVE_DISPLAY)]
        public bool IsActive { get; set; }
    }
}
