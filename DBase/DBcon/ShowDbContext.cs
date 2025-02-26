using JWTAuthServer.Models;
using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels;
using ShowTickets.Ticketmodels.User;

namespace DB.DBcontext
{

    public class ShowDbContext : DbContext
    {
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Stand> Stands { get; set; }
        public DbSet<StandSeat> StandSeats { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowSeat> ShowSeats { get; set; }

        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TicketSellingWindow> ticketSellingWindows { get; set; }

        public ShowDbContext(DbContextOptions<ShowDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Ensures tokens are deleted if user is removed


            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Stands)
                .WithOne(s => s.Venue)
                .HasForeignKey(s => s.VenueId);

            modelBuilder.Entity<Stand>()
                .HasMany(s => s.StandSeats)
                .WithOne(ss => ss.Stand)
                .HasForeignKey(ss => ss.StandId);

            modelBuilder.Entity<Show>()
                .HasMany(s => s.ShowSeats)
                .WithOne(ss => ss.Show)
                .HasForeignKey(ss => ss.ShowId);

            modelBuilder.Entity<ShowSeat>()
                .HasOne(ss => ss.StandSeat)
                .WithMany()
                .HasForeignKey(ss => ss.StandSeatId);

            /*
            CREATE EXTENSION IF NOT EXISTS pgcrypto;
            ALTER TABLE "ShowSeats"
            ALTER COLUMN "RowVersion" SET DEFAULT gen_random_bytes(8);

            */
            modelBuilder.Entity<ShowSeat>(entity =>
            {
                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType("bytea");
            });

            modelBuilder.Entity<TicketSellingWindow>()
            .HasOne(ss => ss.show)
            .WithOne(tw => tw.ticketSellingWindow)
            .HasForeignKey<Show>(ss => ss.ShowId);

            // Seed initial data for Roles, Users, Clients, and UserRoles.
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Admin Role" },
                new Role { Id = 2, Name = "Editor", Description = " Editor Role" },
                new Role { Id = 3, Name = "User", Description = "User Role" }
            );

        }
    }

}
