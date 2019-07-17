using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;

namespace GourmeJunk.Services
{
    public class MenuItemsService
    {
        private readonly IDeletableEntityRepository<MenuItem> menuItems;

        public MenuItemsService(IDeletableEntityRepository<MenuItem> menuItems)
        {
            this.menuItems = menuItems;
        }

        
    }
}
