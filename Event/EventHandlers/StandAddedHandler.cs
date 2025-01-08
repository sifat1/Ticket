using DB.DBcontext;
using ShowTickets.Ticketmodels;
using Ticket.Events;

namespace Ticket.EventHandler
{
    public class StandAddedHandler
    {
        private readonly ShowDbContext _context;
    
        public StandAddedHandler(ShowDbContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(StandAddedEvent standEvent)
        {
            
            var standSeats = new List<StandSeat>();
            for (int i = 1; i <= standEvent.SeatCount; i++)
            {
                standSeats.Add(new StandSeat { SeatNumber = i.ToString(), StandId = standEvent.StandId });
            }

            await _context.StandSeats.AddRangeAsync(standSeats);
            await _context.SaveChangesAsync();
        }
    }
}
