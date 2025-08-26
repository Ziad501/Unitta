using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unitta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordResetCodeHash",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetCodeSentAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetCodeHash",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PasswordResetCodeSentAt",
                table: "AspNetUsers");
        }
    }
}
