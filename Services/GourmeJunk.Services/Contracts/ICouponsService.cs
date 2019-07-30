using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Coupons;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface ICouponsService
    {
        Task<IEnumerable<CouponViewModel>> GetAllAsync();

        Task<bool> CheckIfCouponExistsAsync(string couponId);

        Task CreateCouponAsync(CouponCreateInputModel model, IFormFile image);

        Task<TViewModel> GetCouponModelByIdAsync<TViewModel>(string couponId);
    }
}
