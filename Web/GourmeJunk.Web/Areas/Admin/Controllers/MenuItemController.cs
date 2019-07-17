using GourmeJunk.Services;
using GourmeJunk.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly IMenuItemsService menuItemsService;

        public MenuItemController(IMenuItemsService menuItemsService)
        {
            this.menuItemsService = menuItemsService;
        }

        public async Task<IActionResult> Index()
        {
            var menuItemViewModels = await this.menuItemsService.GetAllAsync();

            return View(menuItemViewModels);
        }
    }
}