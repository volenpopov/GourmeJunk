using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Web.Common;
using GourmeJunk.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoriesService categoriesService;

        public CategoryController(ICategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;
        }

        public async Task<IActionResult> Index()
        {
            var categoriesViewModels = await this.categoriesService.GetAllAsync();

            return View(categoriesViewModels);
        }

        public IActionResult Create()
        {
            return View(new CategoryCreateInputModel());
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Create(CategoryCreateInputModel model)
        {
            var alreadyExists = await this.categoriesService.CheckIfCategoryExistsAsync(model.Name);

            if (alreadyExists)
            {
                model.StatusMessage = string.Format(WebConstants.Error.EntityAlreadyExists, model.Name);

                return View(model);
            }

            await this.categoriesService.CreateCategoryAsync(model);

            return RedirectToAction(nameof(Index));
        }
    }
}