using GourmeJunk.Models.ViewModels.SubCategories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface ISubCategoriesService
    {
        Task<IEnumerable<SubCategoryViewModel>> GetAllAsync();

        Task<bool> CheckIfCategorySubCategoryPairExistsAsync(string subCategoryName, string categoryName);

        Task<SubCategoryCreateViewModel> GetSubCategoryCreateViewModel();
    }
}
