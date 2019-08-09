using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Models.InputModels.Home
{
    public class ShoppingCartBindingModel
    {
        [Required]
        public string MenuItemId { get; set; }                
    }
}
