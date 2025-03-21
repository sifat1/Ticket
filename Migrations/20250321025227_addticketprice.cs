using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ticket.Migrations
{
    /// <inheritdoc />
    public partial class addticketprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VenueId",
                table: "ShowSeats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ShowSeatPrices",
                columns: table => new
                {
                    ShowSeatPriceId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    StandId = table.Column<long>(type: "bigint", nullable: false),
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    VenueId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowSeatPrices", x => x.ShowSeatPriceId);
                    table.ForeignKey(
                        name: "FK_ShowSeatPrices_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "ShowId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowSeatPrices_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "StandId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowSeatPrices_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowSeats_VenueId",
                table: "ShowSeats",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowSeatPrices_ShowId",
                table: "ShowSeatPrices",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowSeatPrices_StandId",
                table: "ShowSeatPrices",
                column: "StandId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowSeatPrices_VenueId",
                table: "ShowSeatPrices",
                column: "VenueId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowSeats_Venues_VenueId",
                table: "ShowSeats",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "VenueId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowSeats_Venues_VenueId",
                table: "ShowSeats");

            migrationBuilder.DropTable(
                name: "ShowSeatPrices");

            migrationBuilder.DropIndex(
                name: "IX_ShowSeats_VenueId",
                table: "ShowSeats");

            migrationBuilder.DropColumn(
                name: "VenueId",
                table: "ShowSeats");
        }
    }
}
