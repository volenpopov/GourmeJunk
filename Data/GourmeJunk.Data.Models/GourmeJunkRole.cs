using GourmeJunk.Data.Common.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace GourmeJunk.Data.Models
{
    public class GourmeJunkRole : IdentityRole, IAuditableEntity, IDeletableEntity
    {
        public GourmeJunkRole() : this(null)
        { }

        public GourmeJunkRole(string name) : base(name)
        { }

        public DateTime CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }        
    }
}
