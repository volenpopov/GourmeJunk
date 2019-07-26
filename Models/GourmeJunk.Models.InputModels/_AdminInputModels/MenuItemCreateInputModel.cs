using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class MenuItemCreateInputModel
    {
        [Required]
        [RegularExpression(ModelConstants.MenuItem.NAME_PATTERN, ErrorMessage = ModelConstants.MenuItem.NAME_PATTERN_ERROR)]
        [StringLength(ModelConstants.MenuItem.NAME_MAX_LENGTH, MinimumLength = ModelConstants.MenuItem.NAME_MIN_LENGTH, ErrorMessage = ModelConstants.NAME_LENGTH_ERROR)]
        public string Name { get; set; }

        [MaxLength(ModelConstants.MenuItem.DESCRIPTION_MAX_LENGTH)]
        public string Description { get; set; }

        [Range(typeof(decimal), ModelConstants.MIN_PRICE, ModelConstants.MAX_PRICE, ErrorMessage = ModelConstants.PRICE_ERROR)]
        public decimal Price { get; set; }

        public string Image { get; set; }

        [Required(ErrorMessage = ModelConstants.MUST_SELECT_CATEGORY)]
        public string CategoryId { get; set; }

        public string SubCategoryId { get; set; }
    }
}
