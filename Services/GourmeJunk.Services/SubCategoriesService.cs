using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.SubCategories;
using GourmeJunk.Services.Common;
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
                .AnyAsync(subCategory => subCategory.Name == subCategoryName 
                          && subCategory.CategoryId == categoryId);
        }

        public async Task<SubCategoryCreateViewModel> GetSubCategoryCreateViewModelAsync()
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
            var subCategory = await this.subCategories
                .AllWithDeleted()
                .SingleOrDefaultAsync(subCateg => subCateg.Name == model.Name 
                                      && subCateg.CategoryId == model.CategoryId);
            if (subCategory == null)
            {
                subCategory = new SubCategory
                {
                    Name = model.Name,
                    CategoryId = model.CategoryId
                };

                await this.subCategories.AddAsync(subCategory);
            }
            else if (subCategory.IsDeleted)
            {
                this.subCategories.Undelete(subCategory);
            }

            await this.subCategories.SaveChangesAsync();
        }
       
        public async Task<SubCategoryEditViewModel> GetSubCategoryEditViewModelAsync(string subCategoryId)
        {
            var subCategory = await this.GetSubCategoryByIdAsync(subCategoryId);

            if (subCategory.IsDeleted)
            {
                throw new InvalidOperationException(string.Format(ServicesDataConstants.GetDeletedEntity, nameof(SubCategory), subCategory.Name));
            }

            var subCategoriesList = await this.GetSubCategoriesOfACategoryAsync(subCategory.CategoryId);

            var subCategoryEditViewModel = new SubCategoryEditViewModel
            {
                Id = subCategoryId,
                Name = subCategory.Name,
                CategoryId = subCategory.CategoryId,
                CategoryName = subCategory.Category.Name,
                SubCategoryList = subCategoriesList
            };

            return subCategoryEditViewModel;
        }

        public async Task EditSubCategoryAsync(SubCategoryEditInputModel model)
        {
            var subCategory = await this.GetSubCategoryByIdAsync(model.Id);

            if (subCategory.IsDeleted)
            {
                this.subCategories.Undelete(subCategory);
            }
            else
            {
                subCategory.Name = model.Name;
            }

            await this.subCategories.SaveChangesAsync();
        }

        public async Task<string[]> GetSubCategoriesOfACategoryAsync(string categoryId)
        {
            var subCategoriesNames = await this.subCategories
                .AllAsNoTracking()
                .Where(subCategory => subCategory.CategoryId == categoryId)
                .Select(subCategory => subCategory.Name)
                .ToArrayAsync();

            return subCategoriesNames;
        }

        public async Task<SubCategoryViewModel> GetSubCategoryViewModel(string subCategoryId)
        {
            var subCategory = await this.GetSubCategoryByIdAsync(subCategoryId);

            if (subCategory.IsDeleted)
            {
                throw new InvalidOperationException(string.Format(ServicesDataConstants.GetDeletedEntity, nameof(SubCategory), subCategory.Name));
            }

            var subCategoryViewModel = new SubCategoryViewModel
            {
                Id = subCategoryId,
                Name = subCategory.Name,
                CategoryName = subCategory.Category.Name
            };

            return subCategoryViewModel;
        }

        public async Task DeleteSubCategoryAsync(string subCategoryId)
        {
            var subCategory = await this.GetSubCategoryByIdAsync(subCategoryId);

            if (subCategory.IsDeleted)
            {
                return;
            }

            this.subCategories.Delete(subCategory);
            await this.subCategories.SaveChangesAsync();
        }

        private async Task<SubCategory> GetSubCategoryByIdAsync(string subCategoryId)
        {
            var subCategory = await this.subCategories
                .AllWithDeleted()
                .Include(subCateg => subCateg.Category)
                .SingleOrDefaultAsync(subCateg => subCateg.Id == subCategoryId);

            if (subCategory == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceId, nameof(SubCategory), subCategoryId));
            }

            return subCategory;
        }      
    }
}
