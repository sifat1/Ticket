using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ticket.Migrations
{
    /// <inheritdoc />
    public partial class addticketpricemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowSeatPrices");

            migrationBuilder.CreateTable(
                name: "ShowStandPrice",
                columns: table => new
                {
                    ShowStandPriceId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    VenueId = table.Column<long>(type: "bigint", nullable: false),
                    StandId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowStandPrice", x => x.ShowStandPriceId);
                    table.ForeignKey(
                        name: "FK_ShowStandPrice_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "ShowId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowStandPrice_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "StandId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowStandPrice_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowStandPrice_ShowId",
                table: "ShowStandPrice",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowStandPrice_StandId",
                table: "ShowStandPrice",
                column: "StandId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowStandPrice_VenueId",
                table: "ShowStandPrice",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowStandPrice");

            migrationBuilder.CreateTable(
                name: "ShowSeatPrices",
                columns: table => new
                {
                    ShowSeatPriceId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    StandId = table.Column<long>(type: "bigint", nullable: false),
                    VenueId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false)
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
        }
    }
}
