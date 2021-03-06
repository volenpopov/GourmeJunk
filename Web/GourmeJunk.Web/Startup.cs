﻿using GourmeJunk.Data;
using GourmeJunk.Data.Common;
using GourmeJunk.Data.Common.Repositories;
using GourmeJunk.Data.Models;
using GourmeJunk.Data.Repositories;
using GourmeJunk.Data.Seeding;
using GourmeJunk.Models.ViewModels.Categories;
using GourmeJunk.Services;
using GourmeJunk.Services.Contracts;
using GourmeJunk.Services.Mapping;
using GourmeJunk.Web.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
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
                .AddDefaultTokenProviders();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            services.AddMvc(options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });

            services.Configure<StripeSettings>(this.Configuration.GetSection(WebConstants.Stripe.STRIPE_SECTION_NAME));

            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<EmailOptions>(this.Configuration);            

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
            services.AddScoped<IOrdersService, OrdersService>();

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = this.Configuration[WebConstants.Facebook.FB_APPID_SECTION];
                facebookOptions.AppSecret = this.Configuration[WebConstants.Facebook.FB_APP_SECRET_SECTION];
            });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = this.Configuration[WebConstants.Google.GOOGLE_CLIENTID_SECTION];
                googleOptions.ClientSecret = this.Configuration[WebConstants.Google.GOOGLE_CLIENTSECRET_SECTION];
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            StripeConfiguration.ApiKey = Configuration
                .GetSection(WebConstants.Stripe.STRIPE_SECTION_NAME)[WebConstants.Stripe.SECRET_KEY_SECTION_NAME];

            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
               endpoints =>
               {                   
                   endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                   endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                   endpoints.MapRazorPages();
               });
        }
    }
}
