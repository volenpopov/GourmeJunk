using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GourmeJunk.Data.Common.Models
{
    public class BaseModel<TKey> : IAuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
