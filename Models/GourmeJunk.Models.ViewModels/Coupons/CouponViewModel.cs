using GourmeJunk.Data.Models;
using GourmeJunk.Models.Common;
using GourmeJunk.Services.Mapping;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.Coupons
{
    public class CouponViewModel : BaseCouponViewModel
    {
        public string Id { get; set; }

        public CouponType CouponType { get; set;}
    }
}
