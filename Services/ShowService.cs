using DB.DBcontext;
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

        public async Task<List<Stand>> GetShowStand(int ShowId)
        {
            _logger.LogInformation("Getting stands for show with ID: {ShowId}", ShowId);
            var stands = await _context.ShowSeats.Where(s => s.ShowId == ShowId)
                        .Select(s => s.Stand).Distinct().ToListAsync();
            return stands;
        }

        public async Task<List<ShowTicketPriceDTO>> UnpaidReservations()
        {
            var reservations = await _context.SeatReservations
                .Where(r => r.IsPaid == false && r.ExpirationTime > DateTime.UtcNow)
                .ToListAsync();

            var unpaidReservations = reservations.Select(r => new ShowTicketPriceDTO
            {
                ShowSeatId = r.ShowSeatId,
                ShowId = r.ShowId,
                VenueId = r.VenueId,
                StandId = r.StandId,
                Price = r.Price
            }).ToList();

            return unpaidReservations;
        }

        public async Task<List<Show>> GetShowsAsync()
        {
            _logger.LogInformation("Getting all shows.");
            return await _context.Shows.Include(s => s.Venue).ToListAsync();
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
            _logger.LogInformation("Getting shows with open ticket sales.");
            var localNow = DateTime.UtcNow; // Convert to local time, making it timestamp

            List<Show> selectedshow = await _context.ticketSellingWindows.Where(
                    p => p.enddate > localNow && p.startdate < localNow)
                .Select(p => p.Show).ToListAsync();

            return selectedshow;
        }

        public async Task<object> GetTicketPrice(List<ShowTicketPriceDTO> tickets)
        {
            try
            {

                decimal Totalprice = 0;
                _logger.LogInformation("Getting ticket prices for {tickets}", tickets);
                foreach (var ticket in tickets)
                {
                    var price = await _context.ShowStandPrice
                        .Where(p => p.ShowId == ticket.ShowId && p.VenueId == ticket.VenueId && p.StandId == ticket.StandId)
                        .Select(p => p.Price)
                        .FirstOrDefaultAsync();

                    ticket.Price = price;

                    Totalprice += price;
                }

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
                _logger.LogInformation("Reserving seats for {seatIds}", seatIds);

                foreach (var ticket in seatIds)
                {
                    var price = _context.ShowStandPrice
                        .Where(p => p.ShowId == ticket.ShowId && p.VenueId == ticket.VenueId && p.StandId == ticket.StandId)
                        .Select(p => p.Price)
                        .FirstOrDefault();

                    ticket.Price = price;
                }


                if (seatIds == null || seatIds.Count == 0)
                {
                    return new Exception("No seats to reserve.");
                }

                var expirationTime = DateTime.UtcNow.AddMinutes(10); // Hold for 10 minutes

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
                _logger.LogInformation("Confirming payment for {seatIds}", seatIds);
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

                await _context.ShowSeats.Where(
                    s => reservations.Select(r => r.ShowSeatId).Contains(s.ShowSeatId)
                ).ExecuteUpdateAsync(setters => 
                setters.SetProperty(s => s.IsBooked, true)
                .SetProperty(s => s.UserId, 1)
                .SetProperty(s => s.BookingTime, DateTime.UtcNow)
                );

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
