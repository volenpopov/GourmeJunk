using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Models.ViewModels.SubCategories;
using GourmeJunk.Services.Common;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GourmeJunk.Services
{
    public class SubCategoriesService : ISubCategoriesService
    {
        private readonly IDeletableEntityRepository<SubCategory> subCategoriesRepository;

        public SubCategoriesService(IDeletableEntityRepository<SubCategory> subCategoriesRepository)
        {
            this.subCategoriesRepository = subCategoriesRepository;
        }

        public async Task<IEnumerable<SubCategoryViewModel>> GetAllSubCategoriesViewModelsAsync()
        {
            var subCategoriesViewModels = await this.subCategoriesRepository
                .AllAsNoTracking()                
                .Include(subCategory => subCategory.Category)
                .To<SubCategoryViewModel>()
                .ToArrayAsync();

            return subCategoriesViewModels;
        }

        public async Task<bool> CheckIfCategorySubCategoryPairExistsAsync(string subCategoryName, string categoryId)
        {
            return await this.subCategoriesRepository
                .AllAsNoTracking()
                .AnyAsync(subCategory => subCategory.Name == subCategoryName 
                          && subCategory.CategoryId == categoryId);
        }

        public SubCategoryCreateViewModel GetSubCategoryCreateViewModelAsync(IEnumerable<CategoryViewModel> categoryList)
        {            
            var subCategoryCreateViewModel = new SubCategoryCreateViewModel
            {
                CategoryList = categoryList
            };

            return subCategoryCreateViewModel;
        }

        public async Task CreateSubCategoryAsync(SubCategoryCreateInputModel model)
        {
            var subCategory = await this.subCategoriesRepository
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

                await this.subCategoriesRepository.AddAsync(subCategory);
            }
            else if (subCategory.IsDeleted)
            {
                this.subCategoriesRepository.Undelete(subCategory);
            }

            await this.subCategoriesRepository.SaveChangesAsync();
        }
       
        public async Task<SubCategoryEditViewModel> GetSubCategoryEditViewModelAsync(string subCategoryId)
        {
            var subCategory = await this.GetSubCategoryByIdAsync(subCategoryId);

            var subCategoriesList = await this.GetSubCategoriesNamesOfACategoryAsync(subCategory.CategoryId);

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
            var currentSubCategory = await this.GetSubCategoryByIdAsync(model.Id);

            var newSubCategoryAsExistingDeletedSubCategory = await this.subCategoriesRepository
                .AllWithDeleted()
                .SingleOrDefaultAsync(subCateg => subCateg.Name == model.Name && subCateg.CategoryId == model.CategoryId);

            if (newSubCategoryAsExistingDeletedSubCategory != null && newSubCategoryAsExistingDeletedSubCategory.IsDeleted)
            {
                this.subCategoriesRepository.Delete(currentSubCategory);

                this.subCategoriesRepository.Undelete(newSubCategoryAsExistingDeletedSubCategory);
            }
            else
            {
                currentSubCategory.Name = model.Name;
            }

            await this.subCategoriesRepository.SaveChangesAsync();
        }        

        public async Task<IEnumerable<SubCategoryBaseViewModel>> GetSubCategoriesOfACategoryAsync(string categoryId)
        {
            var subCategories = await this.subCategoriesRepository
                .AllAsNoTracking()
                .Where(subCategory => subCategory.CategoryId == categoryId)
                .To<SubCategoryBaseViewModel>()
                .ToArrayAsync();

            return subCategories;
        }

        public async Task<SubCategoryViewModel> GetSubCategoryViewModel(string subCategoryId)
        {
            var subCategory = await this.GetSubCategoryByIdAsync(subCategoryId);

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
            
            this.subCategoriesRepository.Delete(subCategory);

            await this.subCategoriesRepository.SaveChangesAsync();
        }

        private async Task<string[]> GetSubCategoriesNamesOfACategoryAsync(string categoryId)
        {
            var subCategoriesNames = await this.subCategoriesRepository
                .AllAsNoTracking()
                .Where(subCategory => subCategory.CategoryId == categoryId)
                .Select(subCategory => subCategory.Name)
                .ToArrayAsync();

            return subCategoriesNames;
        }

        private async Task<SubCategory> GetSubCategoryByIdAsync(string subCategoryId)
        {
            var subCategory = await this.subCategoriesRepository
                .All()
                .Include(subCateg => subCateg.Category)
                .SingleOrDefaultAsync(subCateg => subCateg.Id == subCategoryId);

            if (subCategory == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(SubCategory), subCategoryId));
            }

            return subCategory;
        }
        
    }
}
