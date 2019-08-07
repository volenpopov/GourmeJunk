using GourmeJunk.Common;
using GourmeJunk.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace GourmeJunk.Data.Seeding
{
    public class ApplicationDbContextSeeder
    {
        public static void Seed(GourmeJunkDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var roleManager = serviceProvider.GetRequiredService<RoleManager<GourmeJunkRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<GourmeJunkUser>>();

            Seed(dbContext, roleManager, userManager);
        }

        public static void Seed(GourmeJunkDbContext dbContext, RoleManager<GourmeJunkRole> roleManager, UserManager<GourmeJunkUser> userManager)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (roleManager == null)
            {
                throw new ArgumentNullException(nameof(roleManager));
            }

            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            SeedRoles(roleManager);
            SeedAdmin(dbContext, userManager);
        }

        private static void SeedRoles(RoleManager<GourmeJunkRole> roleManager)
        {
            SeedRole(GlobalConstants.ADMINISTRATOR_ROLE_NAME, roleManager);
            SeedRole(GlobalConstants.KITCHEN_ROLE_NAME, roleManager);
            SeedRole(GlobalConstants.DELIVERY_ROLE_NAME, roleManager);
            SeedRole(GlobalConstants.CUSTOMER_ROLE_NAME, roleManager);
        }

        private static void SeedRole(string roleName, RoleManager<GourmeJunkRole> roleManager)
        {
            var role = roleManager.FindByNameAsync(roleName).GetAwaiter().GetResult();

            if (role == null)
            {
                var result = roleManager.CreateAsync(new GourmeJunkRole(roleName)).GetAwaiter().GetResult();

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }
        }

        private static void SeedAdmin(GourmeJunkDbContext dbContext, UserManager<GourmeJunkUser> userManager)
        {
            //Username and Email have to be the same, otherwise you cannot login with the seeded user
            var user = new GourmeJunkUser
            {
                UserName = GlobalConstants.ADMINISTRATOR__EMAIL,
                Email = GlobalConstants.ADMINISTRATOR__EMAIL,
                FirstName = GlobalConstants.ADMINISTRATOR__NAME,
                LastName = GlobalConstants.ADMINISTRATOR__NAME,
                Address = GlobalConstants.ADMINISTRATOR__NAME,
            };

            userManager.CreateAsync(user, GlobalConstants.ADMINISTRATOR_PASSWORD);
          
            userManager.AddToRoleAsync(user, GlobalConstants.ADMINISTRATOR_ROLE_NAME).GetAwaiter().GetResult();
        }

    }
}
