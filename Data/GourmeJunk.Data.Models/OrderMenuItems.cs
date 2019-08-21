using System;
using System.ComponentModel.DataAnnotations;
using GourmeJunk.Data.Common.Models;

namespace GourmeJunk.Data.Models
{
    public class OrderMenuItems : IAuditableEntity
    {
        [Required]
        public string OrderId { get; set; }

        public virtual Order Order { get; set; }

        [Required]
        public string MenuItemId { get; set; }

        public virtual MenuItem MenuItem { get; set; }

        public int Count { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }
}
