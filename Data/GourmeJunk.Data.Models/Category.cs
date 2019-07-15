using GourmeJunk.Data.Common.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GourmeJunk.Data.Models
{
    public class Category : BaseDeletableModel<string>
    {
        public Category()
        {
            this.SubCategories = new HashSet<SubCategory>();
        }

        [Required]
        [MaxLength(DataModelConstants.CategoryMaxLength)]      
        public string Name { get; set; }

        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}
