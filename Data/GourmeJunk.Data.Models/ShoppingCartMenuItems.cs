using GourmeJunk.Data.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class ShoppingCartMenuItems
    {
        public ShoppingCartMenuItems()
        {
            this.Count = 1;
        }

        [Required]
        public string ShoppingCartId { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }

        [Required]
        public string MenuItemId { get; set; }

        public MenuItem MenuItem { get; set; }

        [Range(DataModelConstants.CART_MIN_ITEMS_COUNT, int.MaxValue, ErrorMessage = DataModelConstants.CART_ITEMS_COUNT_RANGE_ERRORMSG)]
        public int Count { get; set; }
    }
}
