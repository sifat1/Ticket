using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticket.Migrations
{
    /// <inheritdoc />
    public partial class fixshowseat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StandId",
                table: "ShowSeats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ShowSeats_StandId",
                table: "ShowSeats",
                column: "StandId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowSeats_Stands_StandId",
                table: "ShowSeats",
                column: "StandId",
                principalTable: "Stands",
                principalColumn: "StandId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowSeats_Stands_StandId",
                table: "ShowSeats");

            migrationBuilder.DropIndex(
                name: "IX_ShowSeats_StandId",
                table: "ShowSeats");

            migrationBuilder.DropColumn(
                name: "StandId",
                table: "ShowSeats");
        }
    }
}
