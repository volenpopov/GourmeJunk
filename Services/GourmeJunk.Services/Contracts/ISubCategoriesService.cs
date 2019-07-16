using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.SubCategories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface ISubCategoriesService
    {
        Task<IEnumerable<SubCategoryViewModel>> GetAllAsync();

        Task<bool> CheckIfCategorySubCategoryPairExistsAsync(string subCategoryName, string categoryName);

        Task<SubCategoryCreateViewModel> GetSubCategoryCreateViewModelAsync();

        Task CreateSubCategoryAsync(SubCategoryCreateInputModel model);

        Task<string[]> GetSubCategoriesOfACategoryAsync(string categoryId);

        Task<SubCategoryEditViewModel> GetSubCategoryEditViewModelAsync(string subCategoryId);

        Task EditSubCategoryAsync(SubCategoryEditInputModel model);

        Task<SubCategoryViewModel> GetSubCategoryViewModel(string subCategoryId);

        Task DeleteSubCategoryAsync(string subCategoryId);
    }
}
