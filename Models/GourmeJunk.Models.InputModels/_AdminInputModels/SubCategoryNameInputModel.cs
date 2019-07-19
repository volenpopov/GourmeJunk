using GourmeJunk.Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class SubCategoryNameInputModel
    {
        [Required]
        [StringLength(ModelConstants.SubCategory.NAME_MAX_LENGTH, MinimumLength = ModelConstants.SubCategory.NAME_MIN_LENGTH, ErrorMessage = ModelConstants.NAME_LENGTH_ERROR)]
        [RegularExpression(ModelConstants.ONLY_CHARACTERS_PATTERN, ErrorMessage = ModelConstants.ONLY_CHARACTERS_ERROR)]
        [DisplayName(ModelConstants.SubCategory.NAME_DISPLAY)]
        public string Name { get; set; }
    }
}
