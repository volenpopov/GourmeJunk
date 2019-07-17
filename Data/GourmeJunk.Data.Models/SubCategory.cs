using GourmeJunk.Data.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class SubCategory : BaseDeletableModel<string>
    {
        [Required]
        [MaxLength(DataModelConstants.SubCategoryNameMaxLength)]
        public string Name { get; set; }

        [Required]        
        public string CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
