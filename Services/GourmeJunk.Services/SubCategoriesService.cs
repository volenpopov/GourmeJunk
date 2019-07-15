using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.SubCategories;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GourmeJunk.Services
{
    public class SubCategoriesService : ISubCategoriesService
    {
        private readonly IRepository<SubCategory> subCategories;
        private readonly ICategoriesService categoriesService;

        public SubCategoriesService(
            IRepository<SubCategory> subCategories,
            ICategoriesService categoriesService)
        {
            this.subCategories = subCategories;
            this.categoriesService = categoriesService;
        }

        public async Task<IEnumerable<SubCategoryViewModel>> GetAllAsync()
        {
            var subCategoriesViewModels = await this.subCategories
                .AllAsNoTracking()
                .Include(subCategory => subCategory.Category)
                .To<SubCategoryViewModel>()
                .ToArrayAsync();

            return subCategoriesViewModels;
        }

        public async Task<bool> CheckIfCategorySubCategoryPairExistsAsync(string subCategoryName, string categoryName)
        {
            return await this.subCategories
                .AllAsNoTracking()
                .Include(subCategory => subCategory.Category)
                .AnyAsync(subCategory => subCategory.Name == subCategoryName 
                          && subCategory.Category.Name == categoryName);
        }

        public async Task<SubCategoryCreateViewModel> GetSubCategoryCreateViewModel()
        {
            var categoryList = await this.categoriesService.GetAllAsync();

            var subCategoryList = await this.subCategories
                .AllAsNoTracking()
                .Select(subCategory => subCategory.Name)
                .OrderBy(subCategory => subCategory)
                .ToArrayAsync();

            var subCategoryCreateViewModel = new SubCategoryCreateViewModel
            {
                CategoryList = categoryList,
                SubCategoryList = subCategoryList
            };

            return subCategoryCreateViewModel;
        }
    }
}
