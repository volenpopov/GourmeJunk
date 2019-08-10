using GourmeJunk.Data;
using GourmeJunk.Data.Common;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Data.Repositories;
using GourmeJunk.Data.Seeding;
using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Services;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace GourmeJunk.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<GourmeJunkDbContext>(options =>
                    options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;                
                options.IdleTimeout = TimeSpan.FromDays(1);
            });

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
                .AddDefaultTokenProviders()
                .AddDefaultUI(UIFramework.Bootstrap4);

            services.AddMvc(options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });

            //services.AddSingleton(this.Configuration);

            // Identity stores
            services.AddTransient<IUserStore<GourmeJunkUser>, GourmeJunkUserStore>();
            services.AddTransient<IRoleStore<GourmeJunkRole>, GourmeJunkRoleStore>();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            //Application Services
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<ISubCategoriesService, SubCategoriesService>();
            services.AddScoped<IMenuItemsService, MenuItemsService>();
            services.AddScoped<ICouponsService, CouponsService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(CategoryViewModel).GetTypeInfo().Assembly);

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<GourmeJunkDbContext>();

                if (env.IsDevelopment())
                {
                    dbContext.Database.Migrate();
                }

                ApplicationDbContextSeeder.Seed(dbContext, serviceScope.ServiceProvider);

                //TODO: Seed sample data
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");               
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
