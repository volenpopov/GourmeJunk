using GourmeJunk.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace GourmeJunk.Data
{
    public class GourmeJunkRoleStore : 
        RoleStore <
                    GourmeJunkRole,
                    GourmeJunkDbContext,
                    string,
                    IdentityUserRole<string>,
                    IdentityRoleClaim<string>
                  >
    {
        public GourmeJunkRoleStore(GourmeJunkDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        { }

        protected override IdentityRoleClaim<string> CreateRoleClaim(GourmeJunkRole role, Claim claim) =>
            new IdentityRoleClaim<string>
            {
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
            };
    }
}
