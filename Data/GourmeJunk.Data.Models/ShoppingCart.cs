using GourmeJunk.Data.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class ShoppingCart : BaseModel<string>
    {
        public ShoppingCart()
        {
            this.Count = 1;
        }

        [Required]
        public string UserId { get; set; }

        public virtual GourmeJunkUser User { get; set; }

        [Required]
        public string MenuItemId { get; set; }

        public virtual MenuItem MenuItem { get; set; }

        [Range(DataModelConstants.CART_MIN_ITEMS_COUNT, int.MaxValue, ErrorMessage = DataModelConstants.CART_ITEMS_COUNT_RANGE_ERRORMSG)]
        public int Count { get; set; }
    }
}
