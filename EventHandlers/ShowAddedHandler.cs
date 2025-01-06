
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
    }
}
