using GourmeJunk.Models.ViewModels.SubCategories;
using GourmeJunk.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GourmeJunk.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoriesService subCategoriesService;
        

        public SubCategoryController(ISubCategoriesService subCategoriesService)
        {
            this.subCategoriesService = subCategoriesService;            
        }

        public async Task<IActionResult> Index()
        {
            var subCategoryViewModels = await this.subCategoriesService.GetAllAsync();

            return View(subCategoryViewModels);
        }

        public async Task<IActionResult> Create()
        {
            var subCategoryCreateViewModel = await this.subCategoriesService.GetSubCategoryCreateViewModel();

            return View(subCategoryCreateViewModel);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create()
        //{

        //}
    }
}