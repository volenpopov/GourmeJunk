using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Web.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoriesService subCategoriesService;
        private readonly ICategoriesService categoriesService;

        public SubCategoryController(
            ISubCategoriesService subCategoriesService,
            ICategoriesService categoriesService)
        {
            this.subCategoriesService = subCategoriesService;
            this.categoriesService = categoriesService;
        }

        public async Task<IActionResult> Index()
        {
            var subCategoryViewModels = await this.subCategoriesService.GetAllAsync();

            return View(subCategoryViewModels);
        }

        public async Task<IActionResult> Create()
        {
            var subCategoryCreateViewModel = await this.subCategoriesService.GetSubCategoryCreateViewModelAsync ();

            return View(subCategoryCreateViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SubCategoryCreateInputModel model)
        {
            var pairAlreadyExists = await this.subCategoriesService
                .CheckIfCategorySubCategoryPairExistsAsync(model.Name, model.CategoryId);

            if (pairAlreadyExists || !ModelState.IsValid)
            {
                var subCategoryCreateViewModel = await this.subCategoriesService.GetSubCategoryCreateViewModelAsync();

                if (pairAlreadyExists)
                {
                    var categoryName = await this.categoriesService.GetCategoryNameById(model.CategoryId);

                    subCategoryCreateViewModel.StatusMessage = string.Format(WebConstants.Error.EntityAlreadyExists, $"{model.Name} - {categoryName}");
                }

                return View(subCategoryCreateViewModel);
            }

            await this.subCategoriesService.CreateSubCategoryAsync(model);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var subCategoryEditViewModel = await this.subCategoriesService.GetSubCategoryEditViewModelAsync(id);

            return View(subCategoryEditViewModel);
        }

        public async Task<IActionResult> GetSubCategories(string id)
        {
            var subCategoriesNames = await this.subCategoriesService.GetSubCategoriesOfACategoryAsync(id);

            return Json(new SelectList(subCategoriesNames));
        }
    }
}