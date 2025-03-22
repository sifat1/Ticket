using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ticket.Migrations
{
    /// <inheritdoc />
    public partial class reserveseates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeatReservations",
                columns: table => new
                {
                    SeatReservationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShowSeatId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    VenueId = table.Column<long>(type: "bigint", nullable: false),
                    StandId = table.Column<long>(type: "bigint", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatReservations", x => x.SeatReservationId);
                    table.ForeignKey(
                        name: "FK_SeatReservations_ShowSeats_ShowSeatId",
                        column: x => x.ShowSeatId,
                        principalTable: "ShowSeats",
                        principalColumn: "ShowSeatId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeatReservations_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "ShowId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeatReservations_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "StandId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeatReservations_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeatReservations_ShowId",
                table: "SeatReservations",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatReservations_ShowSeatId",
                table: "SeatReservations",
                column: "ShowSeatId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatReservations_StandId",
                table: "SeatReservations",
                column: "StandId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatReservations_VenueId",
                table: "SeatReservations",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeatReservations");
        }
    }
}
