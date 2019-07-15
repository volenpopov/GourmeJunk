using System.Collections.Generic;
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

        public SubCategoriesService(IRepository<SubCategory> subCategories)
        {
            this.subCategories = subCategories;
        }

        public async Task<IEnumerable<SubCategoryViewModel>> GetAllAsync()
        {
            var subCategoriesViewModels = await this.subCategories
                .AllAsNoTracking()
                .To<SubCategoryViewModel>()
                .ToArrayAsync();

            return subCategoriesViewModels;
        }
    }
}
