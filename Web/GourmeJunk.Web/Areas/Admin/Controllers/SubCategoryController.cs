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

                    subCategoryCreateViewModel.StatusMessage = string.Format(WebConstants.Error.ENTITY_ALREADY_EXISTS, $"{model.Name} - {categoryName}");
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

        [HttpPost]
        public async Task<IActionResult> Edit(SubCategoryEditInputModel model)
        {
            var alreadyExists = await this.subCategoriesService
                .CheckIfCategorySubCategoryPairExistsAsync(model.Name, model.CategoryId);

            if (alreadyExists || !ModelState.IsValid)
            {
                var subCategoryEditVieWModel = await this.subCategoriesService
                    .GetSubCategoryEditViewModelAsync(model.Id);

                if (alreadyExists)
                {
                    subCategoryEditVieWModel.StatusMessage = 
                        string.Format(WebConstants.Error
                            .ENTITY_ALREADY_EXISTS, $"{model.Name} - {subCategoryEditVieWModel.CategoryName}");
                }

                return View(subCategoryEditVieWModel);
            }

            await this.subCategoriesService.EditSubCategoryAsync(model);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var subCategoryViewModel = await this.subCategoriesService.GetSubCategoryViewModel(id);

            return View(subCategoryViewModel);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var subCategoryViewModel = await this.subCategoriesService.GetSubCategoryViewModel(id);

            return View(subCategoryViewModel);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(string id)
        {
            await this.subCategoriesService.DeleteSubCategoryAsync(id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetSubCategories(string id)
        {
            var subCategoriesNames = await this.subCategoriesService.GetSubCategoriesOfACategoryAsync(id);

            return Json(new SelectList(subCategoriesNames, "Id", "Name"));
        }
    }
}