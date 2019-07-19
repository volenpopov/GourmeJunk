using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Services;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Web.Common;
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

        public async Task<IActionResult> Create()
        {
            var menuItemCreateViewModel = await this.menuItemsService.GetMenuItemCreateViewModel();

            return View(menuItemCreateViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MenuItemCreateInputModel model)
        {
            var alreadyExists = await this.menuItemsService.CheckIfMenuItemExistsAsync(model.Name);

            if (alreadyExists || !ModelState.IsValid)
            {
                var menuItemCreateViewModel = await this.menuItemsService.GetMenuItemCreateViewModel();

                if (alreadyExists)
                {
                    menuItemCreateViewModel.StatusMessage = string.Format(WebConstants.Error.ENTITY_ALREADY_EXISTS, model.Name);
                }

                menuItemCreateViewModel.Name = model.Name;
                menuItemCreateViewModel.Description = model.Description;
                menuItemCreateViewModel.Price = model.Price;
                menuItemCreateViewModel.CategoryId = model.CategoryId;
                menuItemCreateViewModel.SubCategoryId = model.SubCategoryId;

                return View(menuItemCreateViewModel);
            }

            var image = HttpContext.Request.Form.Files.Count > 0
                ? HttpContext.Request.Form.Files[0]
                : null;

            await this.menuItemsService.CreateMenuItemAsync(model, image);            
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var menuItemEditViewModel = await this.menuItemsService.GetMenuItemEditViewModelAsync(id);

            return View(menuItemEditViewModel);
        }

        
    }   
}