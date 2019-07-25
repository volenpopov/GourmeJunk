using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface ICategoriesService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllAsync();

        Task<bool> CheckIfCategoryExistsAsync(string categoryName);

        Task CreateCategoryAsync(CategoryCreateInputModel model);

        Task<TViewModel> GetCategoryModelByIdAsync<TViewModel>(string id);

        Task EditCategoryAsync(CategoryEditInputModel model);

        Task DeleteCategoryAsync(string id);

        Task<string> GetCategoryNameByIdAsync(string id);

        Task<bool> CheckContainsSubCategoryAsync(string categoryId, string subCategoryId);
    }
}
