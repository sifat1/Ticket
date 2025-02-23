using DB.DBcontext;
using Dtos;
using ShowTickets.Ticketmodels;
using Ticket.Events;

namespace App.Services.Manager
{
    public class ShowManagerService
    {
        private readonly ShowDbContext _context;
        private readonly EventPublisher _eventPublisher;
        private readonly ILogger<ShowService> _logger;

        public ShowManagerService(ShowDbContext context, EventPublisher eventPublisher, ILogger<ShowService> logger)
        {
            _context = context;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task AddShowTask(CreateShow show)
        {
            if (show == null)
            {
                throw new ArgumentNullException(nameof(show));
            }

            var _show = new Show();
            _show.Name = show.Name;
            _show.VenueId = show.VenueId;
            _show.Date = show.Date;

            _context.Shows.Add(_show);
            _context.SaveChanges();

            var _datewindow = new TicketSellingWindow();
            _datewindow.startdate = show.startwindow;
            _datewindow.enddate = show.endwindow;
            _datewindow.show = _show;
            _context.ticketSellingWindows.Add(_datewindow);
            _context.SaveChanges();

            var _event = new ShowAddedEvent { ShowId = _show.ShowId };
            await _eventPublisher.PublishAsync(_event);
        }

        public async Task AddStandAsync(CreateStand stand)
        {
            if (stand == null)
            {
                throw new ArgumentNullException(nameof(stand));
            }

            // Explicit property mapping
            var _stand = new Stand
            {
                VenueId = stand.VenueId,
                SeatCount = (int)(stand?.capacity), // Null check before accessing capacity
                Name = stand?.Name // Null check before accessing name
            };

            _context.Stands.Add(_stand);
            _context.SaveChanges();

            // Event publishing with basic error handling
            var event_ = new StandAddedEvent
            {
                StandId = _stand.StandId,
                SeatCount = _stand.SeatCount
            };

            try
            {
                await _eventPublisher.PublishAsync(event_);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error publishing StandAddedEvent: {ex.Message}");
                // Decide on further actions based on the exception (e.g., retry)
            }
        }

    }


}