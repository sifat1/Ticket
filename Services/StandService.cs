using DB.DBcontext;
using ShowTickets.Ticketmodels;
using Ticket.Events;

namespace App.Services
{
    public class StandService
    {
        private readonly ShowDbContext _context;
        private readonly EventPublisher _eventPublisher;

        public StandService(ShowDbContext context, EventPublisher eventPublisher)
        {
            _context = context;
            _eventPublisher = eventPublisher;
        }

        public async Task AddStandAsync(Stand stand)
        {
            _context.Stands.Add(stand);
            await _context.SaveChangesAsync();

            var event_ = new StandAddedEvent { StandId = stand.StandId, SeatCount = stand.SeatCount
    };
    _eventPublisher.PublishAsync(event_);
}
}

}