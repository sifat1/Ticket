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
            // Fetch venue ID associated with the show
            var venueId = await _context.Shows
                .Where(e => e.ShowId == showEvent.ShowId)
                .Select(e => e.VenueId)
                .FirstOrDefaultAsync();

            if (venueId == 0)
            {
                throw new InvalidOperationException($"Venue not found for ShowId: {showEvent.ShowId}");
            }

            // Fetch all stand seats in one query
            var standSeats = await _context.StandSeats
                .Where(seat => _context.Stands
                    .Where(stand => stand.VenueId == venueId)
                    .Select(stand => stand.StandId)
                    .Contains(seat.StandId))
                .ToListAsync();

            if (!standSeats.Any())
            {
                throw new InvalidOperationException($"No stands or stand seats found for VenueId: {venueId}");
            }

            // Map StandSeats to ShowSeats
            var showSeats = standSeats.Select(seat => new ShowSeat
            {
                ShowId = showEvent.ShowId,
                StandSeatId = seat.StandSeatId,
                StandId = seat.StandId,
                VenueId = venueId,
                IsBooked = false
            }).ToList();

            // Add all ShowSeats in bulk
            await _context.ShowSeats.AddRangeAsync(showSeats);

            // Save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Log and handle concurrency exceptions if necessary
                throw new Exception("Concurrency conflict occurred while adding show seats.", ex);
            }
        }
    }
}
