using GourmeJunk.Models.InputModels._AdminInputModels;
using GourmeJunk.Models.ViewModels.MenuItems;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface IMenuItemsService
    {
        Task<IEnumerable<MenuItemViewModel>> GetAllAsync();

        Task<MenuItemCreateViewModel> GetMenuItemCreateViewModel();

        Task<bool> CheckIfMenuItemExistsAsync(string menuItemName);

        Task CreateMenuItemAsync(MenuItemCreateInputModel model, IFormFile formFile);

        Task<MenuItemEditViewModel> GetMenuItemEditViewModelAsync(string id);
    }
}
