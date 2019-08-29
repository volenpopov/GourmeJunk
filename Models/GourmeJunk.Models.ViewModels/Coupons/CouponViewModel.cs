using GourmeJunk.Data.Models;

namespace GourmeJunk.Models.ViewModels.Coupons
{
    public class CouponViewModel : BaseCouponViewModel
    {
        public string Id { get; set; }

        public CouponType CouponType { get; set;}
    }
}
