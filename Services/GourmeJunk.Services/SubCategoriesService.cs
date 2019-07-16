using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.SubCategories;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GourmeJunk.Services
{
    public class SubCategoriesService : ISubCategoriesService
    {
        private readonly IDeletableEntityRepository<SubCategory> subCategories;
        private readonly ICategoriesService categoriesService;

        public SubCategoriesService(
            IDeletableEntityRepository<SubCategory> subCategories,
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

        public async Task<bool> CheckIfCategorySubCategoryPairExistsAsync(string subCategoryName, string categoryId)
        {
            return await this.subCategories
                .AllAsNoTracking()
                .Include(subCategory => subCategory.Category)
                .AnyAsync(subCategory => subCategory.Name == subCategoryName 
                          && subCategory.CategoryId == categoryId);
        }

        public async Task<SubCategoryCreateViewModel> GetSubCategoryCreateViewModel()
        {
            var categoryList = await this.categoriesService.GetAllAsync();
            
            var subCategoryCreateViewModel = new SubCategoryCreateViewModel
            {
                CategoryList = categoryList,
            };

            return subCategoryCreateViewModel;
        }

        public async Task CreateSubCategoryAsync(SubCategoryCreateInputModel model)
        {
            var subCategory = new SubCategory
            {
                Name = model.Name,
                CategoryId = model.CategoryId
            };

            await this.subCategories.AddAsync(subCategory);
            await this.subCategories.SaveChangesAsync();
        }

        public async Task<string[]> GetSubCategoriesOfACategory(string categoryId)
        {
            var subCategoriesNames = await this.subCategories
                .AllAsNoTracking()
                .Include(subCategory => subCategory.Category)
                .Where(subCategory => subCategory.CategoryId == categoryId)
                .Select(subCategory => subCategory.Name)
                .ToArrayAsync();

            return subCategoriesNames;
        }
    }
}
