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

        public async Task<object> GetTicketPrice(List<ShowTicketPriceDTO> tickets)
        {
            try
            {

                decimal Totalprice = 0;

                foreach (var ticket in tickets)
                {
                    var price = _context.ShowStandPrice
                        .Where(p => p.ShowId == ticket.ShowId && p.VenueId == ticket.VenueId && p.StandId == ticket.VenueId)
                        .Select(p => p.Price)
                        .FirstOrDefault();

                    ticket.Price = price;

                    Totalprice += price;
                }
                Console.WriteLine(tickets);

                return new { total = Totalprice, tickets = tickets };
            }
            catch (Exception ex)
            {
                return new { message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<object> ReserveSeats(List<ShowTicketPriceDTO> seatIds)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                foreach (var ticket in seatIds)
                {
                    var price = _context.ShowStandPrice
                        .Where(p => p.ShowId == ticket.ShowId && p.VenueId == ticket.VenueId && p.StandId == ticket.VenueId)
                        .Select(p => p.Price)
                        .FirstOrDefault();

                    ticket.Price = price;
                }


                if (seatIds == null || seatIds.Count == 0)
                {
                    return new Exception("No seats to reserve.");
                }

                var expirationTime = DateTime.UtcNow.AddMinutes(10); // Hold for 10 minutes

                Console.WriteLine(seatIds);

                // Create seat reservation objects
                var seats = seatIds.Select(seatId => new SeatReservation
                {
                    ShowSeatId = seatId.ShowSeatId,
                    ShowId = seatId.ShowId, // Ensure the ShowId is populated correctly
                    VenueId = seatId.VenueId, // Ensure the VenueId is populated correctly
                    StandId = seatId.StandId, // Ensure the StandId is populated correctly
                    UserId = 1, // Use the actual user ID here
                    ExpirationTime = expirationTime,
                    IsPaid = false,
                    Price = seatId.Price, // Ensure the price is set correctly
                    RowVersion = new byte[8] // Initialize for versioning; you might need to handle this properly
                }).ToList();

                // Add reservations to the context
                // Validate seat reservations before adding
                if (seats.Any(seat => seat.ShowSeatId <= 0 || seat.ShowId <= 0 || seat.VenueId <= 0 || seat.StandId <= 0 || seat.Price <= 0))
                {
                    throw new Exception("Invalid seat reservation data. Ensure all fields are populated correctly.");
                }

                _context.SeatReservations.AddRange(seats);


                await _context.SaveChangesAsync();

                // Commit the transaction if everything goes fine
                await transaction.CommitAsync();

                await ConfirmPayment(seats, 1);

                return new { success = "Seats reserved successfully." };
            }
            catch (Exception ex)
            {
                return new { message = ex.Message };
            }
        }


        public async Task<object> ConfirmPayment(List<SeatReservation> seatIds, long userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var reservations = await _context.SeatReservations
                    .Where(s => seatIds.Select(si => si.ShowSeatId).Contains(s.ShowSeatId) && s.UserId == userId && s.IsPaid == false)
                    .ToListAsync();

                if (reservations.Count == 0)
                {
                    return new { message = "Reservation not found or already expired." };
                }

                // Check if all reservations match the expected version
                for (int i = 0; i < reservations.Count; i++)
                {
                    if (!reservations[i].RowVersion.SequenceEqual(seatIds[i].RowVersion))
                    {
                        return new { message = "One or more seat reservations have expired or been taken by another user." };
                    }
                }

                // Confirm payment and mark seats as paid
                foreach (var reservation in reservations)
                {
                    reservation.IsPaid = true;
                }

                await _context.ShowSeats
                .Where(s => seatIds.Select(r => r.ShowSeatId).Contains(s.ShowSeatId) &&
                seatIds.Select(r => r.ShowId).Contains(s.ShowId) &&
                seatIds.Select(r => r.VenueId).Contains(s.VenueId) &&
                seatIds.Select(r => r.StandId).Contains(s.StandId))
                .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.IsBooked, true));

                await _context.SaveChangesAsync();

                await transaction.CommitAsync(); // Commit if everything succeeds

                return new { success = "Payment confirmed, seats booked successfully." };
            }
            catch (Exception ex)
            {
                return new { message = ex.Message };
            }
        }





    }
}
