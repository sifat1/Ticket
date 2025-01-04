using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels;

namespace DB.DBcontext{
using Microsoft.EntityFrameworkCore;

    public class ShowDbContext : DbContext
    {
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Stand> Stands { get; set; }
        public DbSet<StandSeat> StandSeats { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowSeat> ShowSeats { get; set; }

        public ShowDbContext(DbContextOptions<ShowDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                .HasForeignKey(ss => ss.SeatId);
        }
    }

}
