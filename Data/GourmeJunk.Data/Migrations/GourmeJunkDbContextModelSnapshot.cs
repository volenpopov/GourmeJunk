﻿// <auto-generated />
using System;
using GourmeJunk.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GourmeJunk.Data.Migrations
{
    [DbContext(typeof(GourmeJunkDbContext))]
    partial class GourmeJunkDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GourmeJunk.Data.Models.Category", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.Coupon", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CouponType");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<decimal>("Discount");

                    b.Property<byte[]>("Image");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastModifiedOn");

                    b.Property<decimal?>("MinimumOrderAmount");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Coupons");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.GourmeJunkRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastModifiedOn");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.GourmeJunkUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastModifiedOn");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("ShoppingCartId");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("ShoppingCartId")
                        .IsUnique()
                        .HasFilter("[ShoppingCartId] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.MenuItem", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CategoryId")
                        .IsRequired();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("Description");

                    b.Property<string>("Image");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<decimal>("Price");

                    b.Property<string>("SubCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("SubCategoryId");

                    b.ToTable("MenuItems");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.Order", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comments");

                    b.Property<string>("CouponName");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("LastModifiedOn");

                    b.Property<DateTime>("OrderDate");

                    b.Property<int>("OrderStatus");

                    b.Property<decimal>("OrderTotal");

                    b.Property<decimal>("OrderTotalOriginal");

                    b.Property<int>("PaymentStatus");

                    b.Property<string>("PhoneNumber")
                        .IsRequired();

                    b.Property<DateTime>("PickUpDateAndTime");

                    b.Property<string>("PickupName")
                        .IsRequired();

                    b.Property<string>("TransactionId");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.OrderMenuItems", b =>
                {
                    b.Property<string>("OrderId");

                    b.Property<string>("MenuItemId");

                    b.Property<int>("Count");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("LastModifiedOn");

                    b.HasKey("OrderId", "MenuItemId");

                    b.HasIndex("MenuItemId");

                    b.ToTable("OrderMenuItems");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.ShoppingCart", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("LastModifiedOn");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ShoppingCarts");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.ShoppingCartMenuItems", b =>
                {
                    b.Property<string>("ShoppingCartId");

                    b.Property<string>("MenuItemId");

                    b.Property<int>("Count");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastModifiedOn");

                    b.HasKey("ShoppingCartId", "MenuItemId");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("MenuItemId");

                    b.ToTable("ShoppingCartMenuItems");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.SubCategory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CategoryId")
                        .IsRequired();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastModifiedOn");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("IsDeleted");

                    b.ToTable("SubCategories");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.GourmeJunkUser", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.ShoppingCart", "ShoppingCart")
                        .WithOne("User")
                        .HasForeignKey("GourmeJunk.Data.Models.GourmeJunkUser", "ShoppingCartId");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.MenuItem", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GourmeJunk.Data.Models.SubCategory", "SubCategory")
                        .WithMany()
                        .HasForeignKey("SubCategoryId");
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.Order", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.GourmeJunkUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.OrderMenuItems", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.MenuItem", "MenuItem")
                        .WithMany()
                        .HasForeignKey("MenuItemId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GourmeJunk.Data.Models.Order", "Order")
                        .WithMany("OrderMenuItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.ShoppingCartMenuItems", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.MenuItem", "MenuItem")
                        .WithMany()
                        .HasForeignKey("MenuItemId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GourmeJunk.Data.Models.ShoppingCart", "ShoppingCart")
                        .WithMany("ShoppingCartMenuItems")
                        .HasForeignKey("ShoppingCartId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GourmeJunk.Data.Models.SubCategory", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.Category", "Category")
                        .WithMany("SubCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.GourmeJunkRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.GourmeJunkUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.GourmeJunkUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.GourmeJunkRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GourmeJunk.Data.Models.GourmeJunkUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("GourmeJunk.Data.Models.GourmeJunkUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
