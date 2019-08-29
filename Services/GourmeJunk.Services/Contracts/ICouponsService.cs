using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Coupons;
using GourmeJunk.Models.ViewModels.Home;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface ICouponsService
    {
        Task<IEnumerable<CouponViewModel>> GetAllCouponsViewModelsAsync();

        Task<bool> CheckIfCouponExistsAsync(string couponId);

        Task<bool> CheckIfAnotherCouponWithTheSameNameExistsAsync(string couponId, string couponName);

        Task CreateCouponAsync(CouponCreateInputModel model, IFormFile image);

        Task<TViewModel> GetCouponModelByIdAsync<TViewModel>(string couponId);

        Task EditCouponAsync(CouponEditInputModel model, IFormFile image);

        Task DeleteCouponAsync(string id);

        Task<IEnumerable<IndexCouponViewModel>>  GetAllActiveCouponsWithImageIndexCouponsModelsAsync();
    }
}
