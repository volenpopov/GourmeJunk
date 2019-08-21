using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels.Orders
{
    public class OrderSummaryInputModel
    {
        [Required]
        public string UserId { get; set; }

        public string CouponName { get; set; }

        [Range(typeof(decimal), ModelConstants.MIN_PRICE, ModelConstants.MAX_PRICE, ErrorMessage = ModelConstants.PRICE_ERROR)]
        public decimal OrderTotalOriginal { get; set; }

        [Range(typeof(decimal), ModelConstants.MIN_PRICE, ModelConstants.MAX_PRICE, ErrorMessage = ModelConstants.PRICE_ERROR)]
        public decimal OrderTotal { get; set; }

    }
}
