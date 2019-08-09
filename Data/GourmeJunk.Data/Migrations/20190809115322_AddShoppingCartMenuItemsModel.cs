using Microsoft.EntityFrameworkCore.Migrations;

namespace GourmeJunk.Data.Migrations
{
    public partial class AddShoppingCartMenuItemsModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_MenuItems_MenuItemId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_MenuItemId",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "MenuItemId",
                table: "ShoppingCarts");

            migrationBuilder.CreateTable(
                name: "ShoppingCartMenuItems",
                columns: table => new
                {
                    ShoppingCartId = table.Column<string>(nullable: false),
                    MenuItemId = table.Column<string>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartMenuItems", x => new { x.ShoppingCartId, x.MenuItemId });
                    table.ForeignKey(
                        name: "FK_ShoppingCartMenuItems_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingCartMenuItems_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartMenuItems_MenuItemId",
                table: "ShoppingCartMenuItems",
                column: "MenuItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShoppingCartMenuItems");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "ShoppingCarts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MenuItemId",
                table: "ShoppingCarts",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_MenuItemId",
                table: "ShoppingCarts",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_MenuItems_MenuItemId",
                table: "ShoppingCarts",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
