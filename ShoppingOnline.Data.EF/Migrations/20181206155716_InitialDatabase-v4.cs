using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingOnline.Data.EF.Migrations
{
    public partial class InitialDatabasev4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Shippers_ShipperId",
                table: "Bills");

            migrationBuilder.AlterColumn<int>(
                name: "ShipperId",
                table: "Bills",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Shippers_ShipperId",
                table: "Bills",
                column: "ShipperId",
                principalTable: "Shippers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Shippers_ShipperId",
                table: "Bills");

            migrationBuilder.AlterColumn<int>(
                name: "ShipperId",
                table: "Bills",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Shippers_ShipperId",
                table: "Bills",
                column: "ShipperId",
                principalTable: "Shippers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
