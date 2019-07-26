using GourmeJunk.Services.Contracts;
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
    }
}