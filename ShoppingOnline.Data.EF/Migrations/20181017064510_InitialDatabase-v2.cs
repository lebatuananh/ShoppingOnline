using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingOnline.Data.EF.Migrations
{
    public partial class InitialDatabasev2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AppUsers_UserId",
                table: "Announcements");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Announcements",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AppUsers_UserId",
                table: "Announcements",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AppUsers_UserId",
                table: "Announcements");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Announcements",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AppUsers_UserId",
                table: "Announcements",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
