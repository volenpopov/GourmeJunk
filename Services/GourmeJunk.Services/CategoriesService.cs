using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using GourmeJunk.Models.InputModels._AdminInputModels;
using System;
using GourmeJunk.Services.Common;

namespace GourmeJunk.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly IDeletableEntityRepository<Category> categoriesRepository;
        private readonly IDeletableEntityRepository<SubCategory> subCategoriesRepository;

        public CategoriesService(
            IDeletableEntityRepository<Category> categoriesRepository,
            IDeletableEntityRepository<SubCategory> subCategoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.subCategoriesRepository = subCategoriesRepository;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllAsync()
        {
            var categoriesViewModels = await this.categoriesRepository               
                .AllAsNoTracking()
                .To<CategoryViewModel>()
                .ToArrayAsync();

            return categoriesViewModels;
        }

        public async Task<bool> CheckIfCategoryExistsAsync(string categoryName)
        {
            return await this.categoriesRepository
                .AllAsNoTracking()
                .AnyAsync(category => category.Name == categoryName);
        }

        public async Task CreateCategoryAsync(CategoryCreateInputModel model)
        {
            var category = await this.categoriesRepository
                .AllWithDeleted()
                .SingleOrDefaultAsync(categ => categ.Name == model.Name);

            if (category == null)
            {
                category = new Category { Name = model.Name };

                await this.categoriesRepository.AddAsync(category);
            }
            else if (category.IsDeleted)
            {
                this.categoriesRepository.Undelete(category);
            }

            await this.categoriesRepository.SaveChangesAsync();
        }

        public async Task<TViewModel> GetCategoryModelByIdAsync<TViewModel>(string id)
        {
            var category = await this.categoriesRepository
                .AllAsNoTracking()
                .Where(categ => categ.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            if (category == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Category), id));
            }

            return category;
        }

        public async Task EditCategoryAsync(CategoryEditInputModel model)
        { 
            var currentCategory = await GetCategoryByIdAsync(model.Id);

            var newCategoryAsExistingDeletedCategory = await this.categoriesRepository
                .AllWithDeleted()
                .Include(categ => categ.SubCategories)
                .SingleOrDefaultAsync(categ => categ.Name == model.Name);

            if (newCategoryAsExistingDeletedCategory != null && newCategoryAsExistingDeletedCategory.IsDeleted)
            {
                await this.DeleteCategoryAsync(currentCategory.Id);
                this.categoriesRepository.Undelete(newCategoryAsExistingDeletedCategory);
            }
            else
            {
                currentCategory.Name = model.Name;
            }            
                       
            await this.categoriesRepository.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            var category = await this.GetCategoryByIdAsync(id);

            await this.categoriesRepository
                 .ExecuteSqlCommandAsync(ServicesDataConstants.SQL_MODIFY_DELETABLE_ENTITIES_SUBCATEGORIES, category.Id);

            await this.categoriesRepository
                .ExecuteSqlCommandAsync(ServicesDataConstants.SQL_MODIFY_DELETABLE_ENTITIES_MENUITEMS, category.Id);

            this.categoriesRepository.Delete(category);

            await this.categoriesRepository.SaveChangesAsync();
        }

        public async Task<string> GetCategoryNameByIdAsync(string id)
        {
            var category = await this.GetCategoryByIdAsync(id);

            return category.Name;
        }

        public async Task<bool> CheckContainsSubCategoryAsync(string categoryId, string subCategoryId)
        {
            var category = await this.categoriesRepository
                .AllAsNoTracking()
                .Include(categ => categ.SubCategories)
                .SingleOrDefaultAsync(categ => categ.Id == categoryId);

            if (category == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Category), categoryId));
            }

            return category.SubCategories
                .Any(subCategory => subCategory.Id == subCategoryId);
        }

        private async Task<Category> GetCategoryByIdAsync(string id)
        {
            var category = await this.categoriesRepository
                .All()
                .SingleOrDefaultAsync(categ => categ.Id == id);

            if (category == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NULL_REFERENCE_ID, nameof(Category), id));
            }

            return category;
        }
    }
}
