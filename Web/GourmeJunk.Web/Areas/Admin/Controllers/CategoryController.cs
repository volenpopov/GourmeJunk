using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Services.Contracts;
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
    }
}