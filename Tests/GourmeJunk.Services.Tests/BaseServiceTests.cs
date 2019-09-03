using GourmeJunk.Data;
using GourmeJunk.Data.Common;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Data.Repositories;
using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using GourmeJunk.Web.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace GourmeJunk.Services.Tests
{
    public abstract class BaseServiceTests : IDisposable
    {
        protected IServiceProvider ServiceProvider { get; set; }

        protected GourmeJunkDbContext DbContext { get; set; }
        
        protected BaseServiceTests()
        {
            var services = this.SetServices();

            this.ServiceProvider = services.BuildServiceProvider();
            this.DbContext = this.ServiceProvider.GetRequiredService<GourmeJunkDbContext>();
        }

        private ServiceCollection SetServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<GourmeJunkDbContext>(
                opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentity<GourmeJunkUser, GourmeJunkRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<GourmeJunkDbContext>()
                .AddUserStore<GourmeJunkUserStore>()
                .AddRoleStore<GourmeJunkRoleStore>()
                .AddDefaultTokenProviders();

            // Identity stores
            services.AddTransient<IUserStore<GourmeJunkUser>, GourmeJunkUserStore>();
            services.AddTransient<IRoleStore<GourmeJunkRole>, GourmeJunkRoleStore>();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            //Application Services
            services.AddSingleton<IEmailSender, EmailSender>();
            
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<ISubCategoriesService, SubCategoriesService>();
            services.AddScoped<IMenuItemsService, MenuItemsService>();
            services.AddScoped<ICouponsService, CouponsService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<IOrdersService, OrdersService>();

            AutoMapperConfig.RegisterMappings(typeof(CategoryViewModel).GetTypeInfo().Assembly);

            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });

            return services;
        }

        public void Dispose()
        {
            this.DbContext.Database.EnsureDeleted();
            this.SetServices();
        }
    }
}
