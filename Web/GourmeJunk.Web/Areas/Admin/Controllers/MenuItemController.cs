using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly

        public MenuItemController()
        {

        }

        public async Task<IActionResult> Index()
        {
            var menuItemsViewModels = 

            return View();
        }
    }
}