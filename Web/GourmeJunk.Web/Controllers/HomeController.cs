using GourmeJunk.Models.ViewModels.Home;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Web.Common;
using GourmeJunk.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICouponsService couponsService;
        private readonly ICategoriesService categoriesService;
        private readonly IMenuItemsService menuItemsService;
        private readonly IShoppingCartService shoppingCartService;

        public HomeController(
            ICouponsService couponsService,
            ICategoriesService categoriesService,
            IMenuItemsService menuItemsService,
            IShoppingCartService shoppingCartService
            )
        {
            this.couponsService = couponsService;
            this.categoriesService = categoriesService;
            this.menuItemsService = menuItemsService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index()
        {
            var indexViewModel = new IndexViewModel()
            {
                Coupons = await this.couponsService.GetAllIndexCouponsModelsAsync(),
                Categories = await this.categoriesService.GetAllAsync(),
                MenuItems = await this.menuItemsService.GetAllIndexMenuItemsModelsAsync()
            };

            return View(indexViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Details(string id)
        {
            var shoppingCartViewModel = await this.shoppingCartService.GetShoppingCartViewModelAsync(id);

            return View(shoppingCartViewModel);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(
            [FromForm(Name = WebConstants.CART_FORM_MENUITEM_ID_FIELDNAME)] string menuItemId,
            int count)
        {            
            if (!ModelState.IsValid)
            {
                var shoppingCartViewModel = await this.shoppingCartService.GetShoppingCartViewModelAsync(menuItemId);

                return View(shoppingCartViewModel);
            }

            var user = (ClaimsIdentity)this.User.Identity;
            
            await this.shoppingCartService.UpdateShoppingCartAsync(menuItemId, count, user);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
