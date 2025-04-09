using DB.DBcontext;
using Dtos;
using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels;
using Ticket.Events;

namespace App.Services.Manager
{
    public class ShowManagerService
    {
        private readonly ShowDbContext _context;
        private readonly EventPublisher _eventPublisher;
        private readonly ILogger<ShowService> _logger;
        private readonly string _ShowThumnilePath = Path.Combine(Directory.GetCurrentDirectory(), "ShowThumnile");


        public ShowManagerService(ShowDbContext context, EventPublisher eventPublisher, ILogger<ShowService> logger)
        {
            _context = context;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task SetTicketOpeningAsync(ShowOpening showOpening)
        {
            if (showOpening == null)
                throw new ArgumentNullException(nameof(showOpening));

            var getshow = await _context.Shows.FirstOrDefaultAsync(ss => ss.ShowId == showOpening.ShowId);

            if (getshow == null)
                throw new ArgumentNullException(nameof(getshow));

            _context.ticketSellingWindows.Add(new TicketSellingWindow
            {
                ShowId = showOpening.ShowId,
                startdate = showOpening.StartDate,
                enddate = showOpening.EndDate
            });

            await _context.SaveChangesAsync();
        }

        public async Task AddShowTask(CreateShow show)
        {

            if (show == null)
            {
                throw new ArgumentNullException(nameof(show));
            }

            if (show.Photo != null && show.Photo.Length > 0)
            {
                if (!Directory.Exists(_ShowThumnilePath))
                {
                    Directory.CreateDirectory(_ShowThumnilePath);
                }

                var photoFileName = $"{Guid.NewGuid()}_{show.Photo.FileName}";
                var photoFilePath = Path.Combine(_ShowThumnilePath, photoFileName);

                using (var stream = new FileStream(photoFilePath, FileMode.Create))
                {
                     show.Photo.CopyTo(stream);
                }

                show.PhotoPath = photoFileName;
            }

            var _show = new Show();
            _show.Name = show.Name;
            _show.VenueId = show.VenueId;
            _show.Date = show.Date;
            _show.Description = show.Description;
            _show.ThumnileFilePath = show.PhotoPath;
            _context.Shows.Add(_show);

            var _datewindow = new TicketSellingWindow();
            _datewindow.startdate = show.startwindow;
            _datewindow.enddate = show.endwindow;
            _datewindow.Show = _show;

            _context.ticketSellingWindows.Add(_datewindow);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            _context.SaveChanges();

            //await SetTicketOpeningAsync(new ShowOpening { ShowId = _show.ShowId, StartDate = show.startwindow, EndDate = show.endwindow });

            
            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                var _event = new ShowAddedEvent { ShowId = _show.ShowId };
                await _eventPublisher.PublishAsync(_event);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error publishing ShowAddedEvent: {ex.Message}");
            }
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

        public async Task AddShowSeatPriceAsync(ShowStandPriceDTO showSeatPrice)
        {
            if (showSeatPrice == null)
            {
                throw new ArgumentNullException(nameof(showSeatPrice));
            }

            var _showSeatPrice = new ShowStandPrice
            {
                Price = showSeatPrice.Price,
                ShowId = showSeatPrice.ShowId,
                VenueId = showSeatPrice.VenueId,
                StandId = showSeatPrice.StandId
            };

            _context.ShowStandPrice.Add(_showSeatPrice);
            _context.SaveChanges();
        }
    }


}