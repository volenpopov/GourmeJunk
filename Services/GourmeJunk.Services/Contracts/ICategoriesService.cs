using GourmeJunk.Models.ViewModels.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface ICategoriesService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllAsync();
    }
}
