using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class SubCategoryEditInputModel : SubCategoryNameInputModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string CategoryId { get; set; }
    }
}
