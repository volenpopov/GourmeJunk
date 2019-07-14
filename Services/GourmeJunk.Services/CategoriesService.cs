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
    }
}
