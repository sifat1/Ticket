
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
            var venues = await _context.Shows.Where(e => e.ShowId 
            == showEvent.ShowId).Select(e => e.VenueId).ToListAsync();

            var stands = await _context.Stands.Where(e => e.VenueId == venues[0]).ToListAsync();

            foreach (var val in stands){
            var standSeats = await _context.StandSeats.Where(e => e.StandId == val.StandId).ToListAsync();
            var showSeats = standSeats.Select(seat => new ShowSeat
            {
                ShowId = showEvent.ShowId,
                StandSeatId = seat.StandSeatId,
                IsBooked = false
            });

            _context.ShowSeats.AddRange(showSeats);
            await _context.SaveChangesAsync();
            }
        }
    }
}
