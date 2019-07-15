using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class SubCategoryCreateInputModel : SubCategoryNameInputModel
    {
        [Required]
        public string CategoryId { get; set; }
    }
}
