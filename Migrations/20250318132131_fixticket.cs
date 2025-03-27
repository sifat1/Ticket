using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ticket.Migrations
{
    /// <inheritdoc />
    public partial class fixticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shows_ticketSellingWindows_ShowId",
                table: "Shows");

            migrationBuilder.AlterColumn<long>(
                name: "ShowId",
                table: "Shows",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_ticketSellingWindows_ShowId",
                table: "ticketSellingWindows",
                column: "ShowId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ticketSellingWindows_Shows_ShowId",
                table: "ticketSellingWindows",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "ShowId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ticketSellingWindows_Shows_ShowId",
                table: "ticketSellingWindows");

            migrationBuilder.DropIndex(
                name: "IX_ticketSellingWindows_ShowId",
                table: "ticketSellingWindows");

            migrationBuilder.AlterColumn<long>(
                name: "ShowId",
                table: "Shows",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_ticketSellingWindows_ShowId",
                table: "Shows",
                column: "ShowId",
                principalTable: "ticketSellingWindows",
                principalColumn: "TicketSellingWindowId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
