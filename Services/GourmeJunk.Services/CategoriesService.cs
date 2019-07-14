using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;

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
                .All()
                .To<CategoryViewModel>()
                .ToArrayAsync();

            return categoriesViewModels;
        }
    }
}
