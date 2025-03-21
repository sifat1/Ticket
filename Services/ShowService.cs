using System.Numerics;
using DB.DBcontext;
using Dtos;
using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels;

namespace App.Services
{
    public class ShowService
    {
        private readonly ShowDbContext _context;
        private readonly EventPublisher _eventPublisher;
        private readonly ILogger<ShowService> _logger;

        public ShowService(ShowDbContext context, EventPublisher eventPublisher, ILogger<ShowService> logger)
        {
            _context = context;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<List<Stand>> getstands(int venueid)
        {
            return await _context.Stands.Where(venue => venue.VenueId == venueid).ToListAsync();
        }


        public async Task<string> BookTicketAsync(BookingRequest request)
        {
            if (request == null || request.ShowId <= 0 || request.SeatId <= 0 || string.IsNullOrEmpty(request.UserId))
                return "Invalid booking request.";

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var showSeat = await _context.ShowSeats
                    .FirstOrDefaultAsync(ss => ss.ShowId == request.ShowId && ss.StandSeatId == request.SeatId);

                if (showSeat == null)
                    return "Seat not found for the selected show.";

                if (showSeat.IsBooked)
                    return "Seat is already booked.";

                // Update booking details
                showSeat.IsBooked = true;
                showSeat.BookingTime = DateTime.UtcNow;
                showSeat.UserId = request.UserId;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Booking successful!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Booking failed: {ex.Message}";
            }
        }

        public async Task<string> CancelBookingAsync(BookingRequest request)
        {
            if (request == null || request.ShowId <= 0 || request.SeatId <= 0 || string.IsNullOrEmpty(request.UserId))
                return "Invalid cancellation request.";

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var showSeat = await _context.ShowSeats
                    .FirstOrDefaultAsync(ss => ss.ShowId == request.ShowId && ss.StandSeatId == request.SeatId);

                if (showSeat == null)
                    return "Seat not found for the selected show.";

                if (!showSeat.IsBooked || showSeat.UserId != request.UserId)
                    return "Cannot cancel: Seat is not booked or belongs to a different user.";

                // Update booking details
                showSeat.IsBooked = false;
                showSeat.BookingTime = null;
                showSeat.UserId = null;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Booking canceled successfully!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Cancellation failed: {ex.Message}";
            }
        }

        public async Task<string> BookTicketWithOptimisticConcurrencyAsync(BookingRequest request)
        {
            if (request == null || request.ShowId <= 0 || request.SeatId <= 0 || string.IsNullOrEmpty(request.UserId))
                return "Invalid booking request.";

            try
            {
                var showSeat = await _context.ShowSeats
                    .FirstOrDefaultAsync(ss => ss.ShowId == request.ShowId && ss.StandSeatId == request.SeatId);

                if (showSeat == null)
                    return "Seat not found for the selected show.";

                if (showSeat.IsBooked)
                    return "Seat is already booked.";

                // Update booking details
                showSeat.IsBooked = true;
                showSeat.BookingTime = DateTime.UtcNow;
                showSeat.UserId = request.UserId;

                await _context.SaveChangesAsync();
                return "Booking successful!";
            }
            catch (DbUpdateConcurrencyException)
            {
                return "Booking failed due to concurrency conflict. Please try again.";
            }
        }

        public async Task<List<Show>> GetShowsAsync()
        {
            return await _context.Shows.Include(s => s.Venue).ToListAsync();
        }

        public async Task AddShowAsync(Show show)
        {
            if (show == null)
                throw new ArgumentNullException(nameof(show));

            await _context.Shows.AddAsync(show);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShowSeat>> GetSeatsAsync(int showId, int StandId)
        {
            if (showId <= 0)
                throw new ArgumentException("Invalid ShowId.", nameof(showId));

            return await _context.ShowSeats
                .Where(ss => ss.ShowId == showId && ss.StandSeat.StandId == StandId)
                .ToListAsync();
        }

        public async Task<List<Show>> GetTicketOpeningAsync()
        {
            var localNow = DateTime.UtcNow; // Convert to local time, making it timestamp
            var getshowsidTask = _context.ticketSellingWindows
                .Where(p => p.enddate > localNow && p.startdate < localNow)
                .Select(p => p.ShowId)
                .ToListAsync();

            var getshowsid = await getshowsidTask; // Await the task to get the List<long>

            var shows = await _context.Shows
                .Include(ss => ss.Venue)
                .Where(ss => getshowsid.Contains(ss.ShowId)) // Use Contains() instead of 'in'
                .ToListAsync();

            return shows;
        }

        public async Task<decimal> GetTicketPrice(List<ShowTicketPriceDTO> tickets)
        {
            decimal Totalprice = 0;

            foreach(var ticket in tickets)
            {
                var price  = _context.ShowStandPrice
                    .Where(p => p.ShowId == ticket.ShowId && p.VenueId == ticket.VenueId && p.StandId == ticket.VenueId)
                    .Select(p => p.Price)
                    .FirstOrDefault();
                
                ticket.Price = price;
                
                Totalprice += price;
            }

            return Totalprice;
        }



    }
}
