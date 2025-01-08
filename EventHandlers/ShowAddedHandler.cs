
using DB.DBcontext;
using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels;
using Ticket.Events;

namespace Ticket.EventHandler
{
    public class ShowAddedHandler
    {
        private readonly ShowDbContext _context;

        public ShowAddedHandler(ShowDbContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(ShowAddedEvent showEvent)
        {
            var standSeats = await _context.StandSeats.ToListAsync();
            var showSeats = standSeats.Select(seat => new ShowSeat
            {
                ShowId = showEvent.ShowId,
                StandSeatId = seat.StandSeatId,
                IsBooked = false
            });

            _context.ShowSeats.AddRange(showSeats);
            await _context.SaveChangesAsync();
        }
        public async Task HandleShowAsync(ShowAddedEvent showEvent)
        {
            // Fetch venue ID associated with the show
            var venueId = await _context.Shows
                .Where(e => e.ShowId == showEvent.ShowId)
                .Select(e => e.VenueId)
                .FirstOrDefaultAsync();

            if (venueId == 0)
            {
                throw new Exception($"Venue not found for ShowId: {showEvent.ShowId}");
            }

            // Fetch all stands and their seats in a single query
            var standSeats = await _context.Stands
                .Where(e => e.VenueId == venueId)
                .SelectMany(stand => _context.StandSeats.Where(seat => seat.StandId == stand.StandId))
                .ToListAsync();

            if (!standSeats.Any())
            {
                throw new Exception($"No stands or stand seats found for VenueId: {venueId}");
            }

            // Map StandSeats to ShowSeats
            var showSeats = standSeats.Select(seat => new ShowSeat
            {
                ShowId = showEvent.ShowId,
                StandSeatId = seat.StandSeatId,
                IsBooked = false
            }).ToList();

            // Add all ShowSeats in bulk
            await _context.ShowSeats.AddRangeAsync(showSeats);

            // Save changes once
            await _context.SaveChangesAsync();
        }

    }
}
