using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Coupons;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Web.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ICouponsService couponsService;

        public CouponController(ICouponsService couponsService)
        {
            this.couponsService = couponsService;
        }

        public async Task<IActionResult> Index()
        {
            var couponsViewModels = await this.couponsService.GetAllAsync();

            return View(couponsViewModels);
        }

        public IActionResult Create()
        {
            return View(new CouponCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponCreateInputModel model)
        {
            var alreadyExists = await this.couponsService.CheckIfCouponExistsAsync(model.Name);

            if (alreadyExists || !ModelState.IsValid)
            {
                var couponCreateViewModel = new CouponCreateViewModel();

                if (alreadyExists)
                {
                    couponCreateViewModel.StatusMessage = string.Format(WebConstants.Error.ENTITY_ALREADY_EXISTS, model.Name);
                }

                couponCreateViewModel.Name = model.Name;
                couponCreateViewModel.CouponType = model.CouponType.ToString();
                couponCreateViewModel.Discount = model.Discount;
                couponCreateViewModel.MinimumOrderAmount = model.MinimumOrderAmount;
                couponCreateViewModel.IsActive = model.IsActive;

                return View(couponCreateViewModel);
            }

            var image = HttpContext.Request.Form.Files.Count > 0
                 ? HttpContext.Request.Form.Files[0]
                 : null;

            await this.couponsService.CreateCouponAsync(model, image);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var couponEditViewModel = await this.couponsService.GetCouponModelByIdAsync<CouponViewModelExtended>(id);                            

            return View(couponEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CouponEditInputModel model)
        {            
            var alreadyExists = await this.couponsService.CheckIfCouponExistsAsync(model.Id, model.Name);

            if (alreadyExists || !ModelState.IsValid)
            {
                var couponEditViewModel = await this.couponsService.GetCouponModelByIdAsync<CouponViewModelExtended>(model.Id);

                if (alreadyExists)
                {
                    couponEditViewModel.StatusMessage =
                        string.Format(WebConstants.Error.ENTITY_ALREADY_EXISTS, $"{model.Name}");
                }

                return View(couponEditViewModel);
            }

            var image = HttpContext.Request.Form.Files.Count > 0
                ? HttpContext.Request.Form.Files[0]
                : null;

            await this.couponsService.EditCouponAsync(model, image);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var couponDetailsModel = await this.couponsService.GetCouponModelByIdAsync<CouponViewModelExtended>(id);

            return View(couponDetailsModel);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var couponDeleteModel = await this.couponsService.GetCouponModelByIdAsync<CouponViewModelExtended>(id);

            return View(couponDeleteModel);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(string id)
        {
            await this.couponsService.DeleteCouponAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}