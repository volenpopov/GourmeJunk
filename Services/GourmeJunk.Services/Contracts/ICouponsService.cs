using GourmeJunk.Models.ViewModels.Coupons;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface ICouponsService
    {
        Task<IEnumerable<CouponViewModel>> GetAllAsync();
    }
}
