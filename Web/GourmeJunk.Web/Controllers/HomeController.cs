using GourmeJunk.Models.ViewModels.Home;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Web.Common;
using GourmeJunk.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IUsersService usersService;

        public HomeController(
            ICouponsService couponsService,
            ICategoriesService categoriesService,
            IMenuItemsService menuItemsService,
            IShoppingCartService shoppingCartService,
            IUsersService usersService
            )
        {
            this.couponsService = couponsService;
            this.categoriesService = categoriesService;
            this.menuItemsService = menuItemsService;
            this.shoppingCartService = shoppingCartService;
            this.usersService = usersService;
        }

        public async Task<IActionResult> Index()
        {
            var indexViewModel = new IndexViewModel()
            {
                Coupons = await this.couponsService.GetAllIndexCouponsModelsAsync(),
                Categories = await this.categoriesService.GetAllAsync(),
                MenuItems = await this.menuItemsService.GetAllIndexMenuItemsModelsAsync()
            };

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;

                var user = await this.usersService.GetUserByIdAsync(userId);

                var userIndividualItemsCount = await this.usersService.GetUserIndividualItemsCount(user.Email);

                HttpContext.Session.SetInt32(WebConstants.SESSION_NAME_SHOPPING_CART_INDIVIDUAL_ITEMS_COUNT, userIndividualItemsCount);
            }

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

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            
            await this.shoppingCartService.UpdateShoppingCartAsync(menuItemId, count, claimsIdentity);

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
