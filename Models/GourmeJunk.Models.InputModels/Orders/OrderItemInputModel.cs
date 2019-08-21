using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels.Orders
{
    public class OrderItemInputModel
    {
        [Required]
        public string Name { get; set; }

        [Range(typeof(decimal), ModelConstants.MIN_PRICE, ModelConstants.MAX_PRICE, ErrorMessage = ModelConstants.PRICE_ERROR)]
        public decimal Price { get; set; }

        [Range(ModelConstants.MIN_ITEM_COUNT, ModelConstants.MAX_ITEM_COUNT)]
        public int Count { get; set; }
    }
}
