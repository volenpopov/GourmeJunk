using System;

namespace GourmeJunk.Data.Common.Models
{
    public class BaseModel<TKey> : IAuditableEntity
    {
        public TKey Id { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
