using GourmeJunk.Models.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class SubCategoryNameInputModel
    {
        [Required]
        [StringLength(ModelConstants.SubCategory.NameMaxLength, MinimumLength = ModelConstants.SubCategory.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        [RegularExpression(ModelConstants.OnlyCharactersPattern, ErrorMessage = ModelConstants.OnlyCharactersError)]
        [DisplayName(ModelConstants.SubCategory.NameDisplay)]
        public string Name { get; set; }
    }
}
