using GourmeJunk.Data.Models;
using GourmeJunk.Services.Mapping;

namespace GourmeJunk.Models.ViewModels.Home
{
    public class IndexCouponViewModel : IMapFrom<Coupon>
    {
        public byte[] Image { get; set; }

        public bool IsActive { get; set; }
    }
}
