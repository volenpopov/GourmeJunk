using GourmeJunk.Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class SubCategoryNameInputModel
    {
        [Required]
        [StringLength(ModelConstants.SubCategory.NameMaxLength, MinimumLength = ModelConstants.SubCategory.NameMinLength, ErrorMessage = ModelConstants.NAME_LENGTH_ERROR)]
        [RegularExpression(ModelConstants.ONLY_CHARACTERS_PATTERN, ErrorMessage = ModelConstants.ONLY_CHARACTERS_ERROR)]
        [DisplayName(ModelConstants.SubCategory.NameDisplay)]
        public string Name { get; set; }
    }
}
