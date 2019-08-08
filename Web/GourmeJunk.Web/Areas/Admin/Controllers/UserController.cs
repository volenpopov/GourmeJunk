using GourmeJunk.Common;
using GourmeJunk.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = GlobalConstants.ADMINISTRATOR_ROLE_NAME)]
    public class UserController : Controller
    {
        private readonly IUsersService usersService;

        public UserController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public async Task<IActionResult> Index()
        {
            var usersViewModels = await this.usersService.GetAllAsync();

            return View(usersViewModels);
        }

        public async Task<IActionResult> Lock(string id)
        {
            await this.usersService.LockUserAsync(id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLock(string id)
        {
            await this.usersService.UnLockUserAsync(id);

            return RedirectToAction(nameof(Index));
        }

    }
}