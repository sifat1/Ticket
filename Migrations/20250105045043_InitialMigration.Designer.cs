﻿// <auto-generated />
using System;
using DB.DBcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ticket.Migrations
{
    [DbContext(typeof(ShowDbContext))]
    [Migration("20250105045043_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

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

                    b.Property<long>("StandSeatId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("ShowSeatId");

                    b.HasIndex("ShowId");

                    b.HasIndex("StandSeatId");

                    b.ToTable("ShowSeats");
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

            modelBuilder.Entity("ShowTickets.Ticketmodels.User", b =>
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

                    b.Property<string>("PhoneNumber")
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

                    b.HasOne("ShowTickets.Ticketmodels.StandSeat", "StandSeat")
                        .WithMany()
                        .HasForeignKey("StandSeatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Show");

                    b.Navigation("StandSeat");
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

            modelBuilder.Entity("ShowTickets.Ticketmodels.Show", b =>
                {
                    b.Navigation("ShowSeats");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.Stand", b =>
                {
                    b.Navigation("StandSeats");
                });

            modelBuilder.Entity("ShowTickets.Ticketmodels.Venue", b =>
                {
                    b.Navigation("Stands");
                });
#pragma warning restore 612, 618
        }
    }
}
