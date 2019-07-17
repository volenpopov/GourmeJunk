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
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceId, nameof(Category), id));
            }

            return category;
        }

        public async Task EditCategoryAsync(CategoryEditInputModel model)
        {
            var category = await GetCategoryById(model.Id);

            if (category.IsDeleted)
            {
                this.categoriesRepository.Undelete(category);
            }
            else
            {
                category.Name = model.Name;
            }            

            await this.categoriesRepository.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            var category = await GetCategoryById(id);

            if (category.IsDeleted)
            {
                return;
            }

            //Set each child SubCategory of Category to IsDeleted = True
            var subCategories = await this.subCategoriesRepository
                .All()
                .Where(subCategory => subCategory.CategoryId == id)
                .ToArrayAsync();

            foreach (var subCategory in subCategories)
            {
                this.subCategoriesRepository.Delete(subCategory);
            }

            this.categoriesRepository.Delete(category);
            await this.categoriesRepository.SaveChangesAsync();
        }

        public async Task<string> GetCategoryNameById(string id)
        {
            var category = await this.GetCategoryById(id);

            return category.Name;
        }

        private async Task<Category> GetCategoryById(string id)
        {
            var category = await this.categoriesRepository
                .AllWithDeleted()
                .SingleOrDefaultAsync(categ => categ.Id == id);

            if (category == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceId, nameof(Category), id));
            }

            return category;
        }
    }
}
