using GourmeJunk.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace GourmeJunk.Data
{
    public class GourmeJunkUserStore : 
        UserStore <
                    GourmeJunkUser,
                    GourmeJunkRole,
                    GourmeJunkDbContext,
                    string,
                    IdentityUserClaim<string>,
                    IdentityUserRole<string>,
                    IdentityUserLogin<string>,
                    IdentityUserToken<string>,
                    IdentityRoleClaim<string>
                  >
    {
        public GourmeJunkUserStore(GourmeJunkDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        { }

        protected override IdentityUserRole<string> CreateUserRole(GourmeJunkUser user, GourmeJunkRole role)
        {
            return new IdentityUserRole<string> { RoleId = role.Id, UserId = user.Id };
        }

        protected override IdentityUserClaim<string> CreateUserClaim(GourmeJunkUser user, Claim claim)
        {
            var identityUserClaim = new IdentityUserClaim<string> { UserId = user.Id };
            identityUserClaim.InitializeFromClaim(claim);
            return identityUserClaim;
        }

        protected override IdentityUserLogin<string> CreateUserLogin(GourmeJunkUser user, UserLoginInfo login) =>
            new IdentityUserLogin<string>
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
            };

        protected override IdentityUserToken<string> CreateUserToken(
            GourmeJunkUser user,
            string loginProvider,
            string name,
            string value)
        {
            var token = new IdentityUserToken<string>
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value,
            };
            return token;
        }
    }
}
