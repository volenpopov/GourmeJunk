using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels._AdminInputModels
{
    public class MenuItemEditInputModel : MenuItemCreateInputModel
    {
        [Required]
        public string Id { get; set; }
    }
}
