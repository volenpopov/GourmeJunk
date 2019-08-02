using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.ViewModels.Coupons
{
    public class CouponCreateViewModel : BaseCouponViewModel
    {                
        public string CouponType { get; set; }               

        public string StatusMessage { get; set; }
    }
}
