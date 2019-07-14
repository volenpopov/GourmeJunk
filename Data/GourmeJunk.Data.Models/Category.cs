using GourmeJunk.Data.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class Category : BaseDeletableModel<string>
    {
        [Required]
        [Display(Name = "Category Name")]        
        public string Name { get; set; }
    }
}
