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
        private readonly IRepository<Category> categoriesRepository;

        public CategoriesService(IRepository<Category> categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
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
            var category = new Category { Name = model.Name };

            await this.categoriesRepository.AddAsync(category);

            await this.categoriesRepository.SaveChangesAsync();
        }

        public async Task<TViewModel> GetCategoryModelByIdAsync<TViewModel>(string id)
        {
            var Category = await this.categoriesRepository
                .AllAsNoTracking()
                .Where(categ => categ.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            if (Category == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceId, nameof(Category), id));
            }

            return Category;
        }

        public async Task EditCategoryAsync(CategoryEditInputModel model)
        {
            var category = await GetCategoryById(model.Id);

            category.Name = model.Name;

            await this.categoriesRepository.SaveChangesAsync();
        }


        //TODO: Check subcategories after the deletion of a category
        public async Task DeleteCategoryAsync(string id)
        {
            var category = await GetCategoryById(id);

            this.categoriesRepository.Delete(category);
            await this.categoriesRepository.SaveChangesAsync();
        }

        private async Task<Category> GetCategoryById(string id)
        {
            var category = await this.categoriesRepository
                .All()
                .SingleOrDefaultAsync(categ => categ.Id == id);

            if (category == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceId, nameof(Data.Models.Category), id));
            }

            return category;
        }
    }
}
