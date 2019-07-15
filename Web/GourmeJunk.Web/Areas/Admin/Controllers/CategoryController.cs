using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Categories;
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
            return View();
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

        public async Task<IActionResult> Edit(string id)
        {
            var categoryEditViewModel = await this.categoriesService.GetCategoryModelByIdAsync<CategoryEditViewModel>(id);

            return View(categoryEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditInputModel model)
        {
            var alreadyExists = await this.categoriesService.CheckIfCategoryExistsAsync(model.Name);

            if (alreadyExists || !ModelState.IsValid)
            {
                var categoryEditViewModel = new CategoryEditViewModel
                {
                    Id = model.Id,
                    Name = model.Name
                };

                if (alreadyExists)
                {
                    categoryEditViewModel.StatusMessage = string.Format(WebConstants.Error.EntityAlreadyExists, model.Name);                    
                }

                return View(categoryEditViewModel);
            }

            await this.categoriesService.EditCategoryAsync(model);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var categoryViewModel = await this.categoriesService.GetCategoryModelByIdAsync<CategoryViewModel>(id);

            return View(categoryViewModel);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var categoryVieWModel = await this.categoriesService.GetCategoryModelByIdAsync<CategoryViewModel>(id);

            return View(categoryVieWModel);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(string id)
        {
            await this.categoriesService.DeleteCategoryAsync(id);
            
            return RedirectToAction(nameof(Index));
        }
    }
}