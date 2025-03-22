﻿// <auto-generated />
using System;
using DB.DBcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ticket.Migrations
{
    [DbContext(typeof(ShowDbContext))]
    partial class ShowDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("JWTAuthServer.Models.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("boolean");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.SeatReservation", b =>
                {
                    b.Property<long>("SeatReservationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("SeatReservationId"));

                    b.Property<DateTime>("ExpirationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<byte[]>("RowVersion")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<long>("ShowId")
                        .HasColumnType("bigint");

                    b.Property<long>("ShowSeatId")
                        .HasColumnType("bigint");

                    b.Property<long>("StandId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("VenueId")
                        .HasColumnType("bigint");

                    b.HasKey("SeatReservationId");

                    b.HasIndex("ShowId");

                    b.HasIndex("ShowSeatId");

                    b.HasIndex("StandId");

                    b.HasIndex("VenueId");

                    b.ToTable("SeatReservations");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.Show", b =>
                {
                    b.Property<long>("ShowId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ShowId"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("VenueId")
                        .HasColumnType("bigint");

                    b.HasKey("ShowId");

                    b.HasIndex("VenueId");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.ShowSeat", b =>
                {
                    b.Property<long>("ShowSeatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ShowSeatId"));

                    b.Property<DateTime?>("BookingTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsBooked")
                        .HasColumnType("boolean");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<long>("ShowId")
                        .HasColumnType("bigint");

                    b.Property<long>("StandId")
                        .HasColumnType("bigint");

                    b.Property<long>("StandSeatId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<long>("VenueId")
                        .HasColumnType("bigint");

                    b.HasKey("ShowSeatId");

                    b.HasIndex("ShowId");

                    b.HasIndex("StandId");

                    b.HasIndex("StandSeatId");

                    b.HasIndex("VenueId");

                    b.ToTable("ShowSeats");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.ShowStandPrice", b =>
                {
                    b.Property<long>("ShowStandPriceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ShowStandPriceId"));

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<long>("ShowId")
                        .HasColumnType("bigint");

                    b.Property<long>("StandId")
                        .HasColumnType("bigint");

                    b.Property<long>("VenueId")
                        .HasColumnType("bigint");

                    b.HasKey("ShowStandPriceId");

                    b.HasIndex("ShowId");

                    b.HasIndex("StandId");

                    b.HasIndex("VenueId");

                    b.ToTable("ShowStandPrice");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.Stand", b =>
                {
                    b.Property<long>("StandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("StandId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SeatCount")
                        .HasColumnType("integer");

                    b.Property<long>("VenueId")
                        .HasColumnType("bigint");

                    b.HasKey("StandId");

                    b.HasIndex("VenueId");

                    b.ToTable("Stands");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.StandSeat", b =>
                {
                    b.Property<long>("StandSeatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("StandSeatId"));

                    b.Property<string>("SeatNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("StandId")
                        .HasColumnType("bigint");

                    b.HasKey("StandSeatId");

                    b.HasIndex("StandId");

                    b.ToTable("StandSeats");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.TicketSellingWindow", b =>
                {
                    b.Property<long>("TicketSellingWindowId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("TicketSellingWindowId"));

                    b.Property<long>("ShowId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("enddate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("startdate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("TicketSellingWindowId");

                    b.HasIndex("ShowId")
                        .IsUnique();

                    b.ToTable("ticketSellingWindows");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.User.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Admin Role",
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Description = " Editor Role",
                            Name = "Editor"
                        },
                        new
                        {
                            Id = 3,
                            Description = "User Role",
                            Name = "User"
                        });
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.User.Users", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.Venue", b =>
                {
                    b.Property<long>("VenueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("VenueId"));

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TotalStands")
                        .HasColumnType("integer");

                    b.HasKey("VenueId");

                    b.ToTable("Venues");
                });

            modelBuilder.Entity("JWTAuthServer.Models.RefreshToken", b =>
                {
                    b.HasOne("ShowTickets.Ticketmodels.User.Users", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.SeatReservation", b =>
                {
                    b.HasOne("ShowTickets.Ticketmodels.Show", "Show")
                        .WithMany()
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShowTickets.Ticketmodels.ShowSeat", "ShowSeat")
                        .WithMany()
                        .HasForeignKey("ShowSeatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShowTickets.Ticketmodels.Stand", "Stand")
                        .WithMany()
                        .HasForeignKey("StandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShowTickets.Ticketmodels.Venue", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Show");

                    b.Navigation("ShowSeat");

                    b.Navigation("Stand");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.Show", b =>
                {
                    b.HasOne("ShowTickets.Ticketmodels.Venue", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.ShowSeat", b =>
                {
                    b.HasOne("ShowTickets.Ticketmodels.Show", "Show")
                        .WithMany("ShowSeats")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShowTickets.Ticketmodels.Stand", "Stand")
                        .WithMany("ShowSeats")
                        .HasForeignKey("StandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShowTickets.Ticketmodels.StandSeat", "StandSeat")
                        .WithMany("ShowSeats")
                        .HasForeignKey("StandSeatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShowTickets.Ticketmodels.Venue", "Venue")
                        .WithMany("ShowSeats")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Show");

                    b.Navigation("Stand");

                    b.Navigation("StandSeat");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.ShowStandPrice", b =>
                {
                    b.HasOne("ShowTickets.Ticketmodels.Show", "Show")
                        .WithMany()
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShowTickets.Ticketmodels.Stand", "Stand")
                        .WithMany()
                        .HasForeignKey("StandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShowTickets.Ticketmodels.Venue", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Show");

                    b.Navigation("Stand");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.Stand", b =>
                {
                    b.HasOne("ShowTickets.Ticketmodels.Venue", "Venue")
                        .WithMany("Stands")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.StandSeat", b =>
                {
                    b.HasOne("ShowTickets.Ticketmodels.Stand", "Stand")
                        .WithMany("StandSeats")
                        .HasForeignKey("StandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stand");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.TicketSellingWindow", b =>
                {
                    b.HasOne("ShowTickets.Ticketmodels.Show", "Show")
                        .WithOne("ticketSellingWindow")
                        .HasForeignKey("ShowTickets.Ticketmodels.TicketSellingWindow", "ShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Show");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.Show", b =>
                {
                    b.Navigation("ShowSeats");

                    b.Navigation("ticketSellingWindow")
                        .IsRequired();
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.Stand", b =>
                {
                    b.Navigation("ShowSeats");

                    b.Navigation("StandSeats");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.StandSeat", b =>
                {
                    b.Navigation("ShowSeats");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.User.Users", b =>
                {
                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.Venue", b =>
                {
                    b.Navigation("ShowSeats");

                    b.Navigation("Stands");
                });
#pragma warning restore 612, 618
        }
    }
}
