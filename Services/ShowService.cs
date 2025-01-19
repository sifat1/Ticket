using System.Net.WebSockets;
using DB.DBcontext;
using Dtos;
using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels;
using Ticket.Events;

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

        public async Task<List<ShowSeat>> GetAvailableSeatsAsync(int showId)
        {
            if (showId <= 0)
                throw new ArgumentException("Invalid ShowId.", nameof(showId));

            return await _context.ShowSeats
                .Where(ss => ss.ShowId == showId && !ss.IsBooked)
                .Include(ss => ss.StandSeat)
                .ToListAsync();
        }
       

    }
}
