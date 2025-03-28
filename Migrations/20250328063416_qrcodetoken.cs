using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticket.Migrations
{
    /// <inheritdoc />
    public partial class qrcodetoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTokenUsed",
                table: "ShowSeats",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "ShowSeats",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenUsedTime",
                table: "ShowSeats",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Shows",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTokenUsed",
                table: "ShowSeats");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "ShowSeats");

            migrationBuilder.DropColumn(
                name: "TokenUsedTime",
                table: "ShowSeats");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Shows");
        }
    }
}
