using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GourmeJunk.Data.Migrations
{
    public partial class AddCouponModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    LastModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 25, nullable: false),
                    CouponType = table.Column<int>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    MinimumOrderAmount = table.Column<decimal>(nullable: true),
                    Image = table.Column<byte[]>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_IsDeleted",
                table: "Coupons",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Name",
                table: "Coupons",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
