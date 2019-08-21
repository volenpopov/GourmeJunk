using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels.Orders
{
    public class OrderInputModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string PickupName { get; set; }

        [Required]
        [RegularExpression(ModelConstants.ONLY_DIGITS_PATTERN, ErrorMessage = ModelConstants.ONLY_DIGITS_ERROR)]
        public string PhoneNumber { get; set; }

        [Required]
        public string PickupDate { get; set; }

        [Required]
        public string PickupTime { get; set; }

        public string Comments { get; set; }

        public string CouponName { get; set; }

        [Range(typeof(decimal), ModelConstants.MIN_PRICE, ModelConstants.MAX_PRICE, ErrorMessage = ModelConstants.PRICE_ERROR)]
        public decimal OrderTotalOriginal { get; set; }
        
        public decimal OrderTotal { get; set; }
    }
}
