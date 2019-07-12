using System;

namespace GourmeJunk.Data.Common.Models
{
    public interface IAuditableEntity
    {
        DateTime CreatedOn { get; set; }
    }
}
