using GourmeJunk.Models.ViewModels.MenuItems;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GourmeJunk.Services.Contracts
{
    public interface IMenuItemsService
    {
        Task<IEnumerable<MenuItemViewModel>> GetAllAsync();
    }
}
