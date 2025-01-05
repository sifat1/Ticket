using DB.DBcontext;
using Dtos;
using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels;


namespace App.services
{
    public class ShowService
    {
        private readonly ShowDbContext _context;

        public ShowService(ShowDbContext context)
        {
            _context = context;
        }

        public async Task<string> BookTicketAsync(BookingRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var showSeat = await _context.ShowSeats
                    .Where(ss => ss.ShowId == request.ShowId && ss.StandSeatId == request.SeatId)
                    .FirstOrDefaultAsync();

                if (showSeat == null)
                {
                    return "Seat not found for the selected show.";
                }

                if (showSeat.IsBooked)
                {
                    return "Seat is already booked.";
                }

                // Update booking details
                showSeat.IsBooked = true;
                showSeat.BookingTime = DateTime.UtcNow;
                showSeat.UserId = request.UserId;

                // Save changes
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();
                return "Booking successful!";
            }
            catch (Exception ex)
            {
                // Rollback transaction on error
                await transaction.RollbackAsync();
                return $"Booking failed: {ex.Message}";
            }
        }


        public async Task<string> CancelBookingAsync(BookingRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var showSeat = await _context.ShowSeats
                    .Where(ss => ss.ShowId == request.ShowId && ss.StandSeatId == request.SeatId)
                    .FirstOrDefaultAsync();

                if (showSeat == null)
                {
                    return "Seat not found for the selected show.";
                }

                if (!showSeat.IsBooked || showSeat.UserId != request.UserId)
                {
                    return "Cannot cancel: Seat is not booked or belongs to a different user.";
                }

                // Update booking details
                showSeat.IsBooked = false;
                showSeat.BookingTime = null;
                showSeat.UserId = null;

                // Save changes
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();
                return "Booking canceled successfully!";
            }
            catch (Exception ex)
            {
                // Rollback transaction on error
                await transaction.RollbackAsync();
                return $"Cancellation failed: {ex.Message}";
            }
        }

        public async Task<string> BookTicketWithOptimisticConcurrencyAsync(BookingRequest request)
        {
            try
            {
                var showSeat = await _context.ShowSeats
                    .Where(ss => ss.ShowId == request.ShowId && ss.StandSeatId == request.SeatId)
                    .FirstOrDefaultAsync();

                if (showSeat == null)
                {
                    return "Seat not found for the selected show.";
                }

                if (showSeat.IsBooked)
                {
                    return "Seat is already booked.";
                }

                // Update booking details
                showSeat.IsBooked = true;
                showSeat.BookingTime = DateTime.UtcNow;
                showSeat.UserId = request.UserId;

                // Save changes with RowVersion check
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
            _context.Shows.Add(show);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShowSeat>> GetAvailableSeatsAsync(int showId)
        {
            return await _context.ShowSeats
                .Where(ss => ss.ShowId == showId && !ss.IsBooked)
                .Include(ss => ss.StandSeat)
                .ToListAsync();
        }
    }
}
