using GourmeJunk.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class SubCategoryCreateInputModel : SubCategoryNameInputModel
    {
        [Required(ErrorMessage = ModelConstants.SubCategory.MustSelectCategory)]
        public string CategoryId { get; set; }
    }
}
