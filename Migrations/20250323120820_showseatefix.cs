using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticket.Migrations
{
    /// <inheritdoc />
    public partial class showseatefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "ShowSeats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShowSeats_UserId",
                table: "ShowSeats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowSeats_Users_UserId",
                table: "ShowSeats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowSeats_Users_UserId",
                table: "ShowSeats");

            migrationBuilder.DropIndex(
                name: "IX_ShowSeats_UserId",
                table: "ShowSeats");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShowSeats",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
