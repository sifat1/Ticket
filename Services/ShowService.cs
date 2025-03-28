using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DB.DBcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShowTickets.Ticketmodels;

namespace App.Services
{
    public class ShowService
    {
        private readonly ShowDbContext _context;
        private readonly EventPublisher _eventPublisher;
        private readonly ILogger<ShowService> _logger;
        private readonly IConfiguration _config;

        public ShowService(ShowDbContext context, EventPublisher eventPublisher, ILogger<ShowService> logger, IConfiguration config)
        {
            _context = context;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _config = config;
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

        public async Task<object> ReserveSeats(List<ShowTicketPriceDTO> seatIds, long userId)
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
                    UserId = userId, // Use the actual user ID here
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

                await ConfirmPayment(seats, userId);

                return new { success = "Seats reserved successfully." };
            }
            catch (Exception ex)
            {
                return new { message = ex.Message };
            }
        }


        public async Task<object> ConfirmPayment(List<SeatReservation> seatIds, long userId)
        {
            if (seatIds == null || seatIds.Count == 0)
            {
                return new { error = "No seats to confirm." };
            }

            if (userId <= 0)
            {
                return new { error = "Invalid user ID." };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Confirming payment for user {userId} with seatIds: {@seatIds}", userId, seatIds);

                var seatIdList = seatIds.Select(si => si.ShowSeatId).ToList();
                var reservations = await _context.SeatReservations
                    .Where(s => seatIdList.Contains(s.ShowSeatId) && !s.IsPaid)
                    .ToListAsync();

                if (reservations.Count == 0)
                {
                    return new { error = "Reservation not found or already expired." };
                }

                // Convert to dictionary for row version check
                var seatIdDict = seatIds.ToDictionary(s => s.ShowSeatId, s => s.RowVersion);

                foreach (var reservation in reservations)
                {
                    if (!seatIdDict.TryGetValue(reservation.ShowSeatId, out var expectedRowVersion) ||
                        !reservation.RowVersion.SequenceEqual(expectedRowVersion))
                    {
                        return new { error = "One or more seat reservations have expired or been taken by another user." };
                    }
                }

                // Mark seats as paid
                foreach (var reservation in reservations)
                {
                    reservation.IsPaid = true;
                }

                var tokens = reservations.ToDictionary(
                    r => r.ShowSeatId,
                    r => GenerateJwtToken(r.ShowSeatId, r.ShowId, userId)
                );

                // Fetch seats to update manually (because ExecuteUpdateAsync can't use dictionary)
                var seatsToUpdate = await _context.ShowSeats.Where(s => seatIdList.Contains(s.ShowSeatId)).ToListAsync();
                foreach (var seat in seatsToUpdate)
                {
                    seat.IsBooked = true;
                    seat.UserId = userId;
                    seat.BookingTime = DateTime.UtcNow;
                    seat.Token = tokens[seat.ShowSeatId]; // Use pre-generated token
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new { success = "Payment confirmed, seats booked successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment for user {userId}", userId);
                await transaction.RollbackAsync();
                return new { error = ex.Message };
            }
        }


        public async Task<object> GenerateQR(long seatId, long userId)
        {
            try
            {
                _logger.LogInformation("Generating QR code for {seatIds}", seatId);
                var seats = await _context.ShowSeats
                    .Where(s => s.ShowSeatId == seatId && s.IsBooked == true)
                    .Select(s => s.Token)
                    .FirstAsync();

                if (seats.Count() == 0)
                {
                    return new { message = "Reservation not found or already expired." };
                }

                var qrCodes = new List<string>();

                foreach (var s in seats)
                {
                    var qrCode = Guid.NewGuid().ToString();
                    //reservation.QRCode = qrCode;
                    qrCodes.Add(qrCode);
                }


                return new { success = "QR codes generated successfully.", qrCodes };
            }
            catch (Exception ex)
            {
                return new { message = ex.Message };
            }

        }

        private string GenerateJwtToken(long ShowSeatId, long ShowId, long userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["qrsecret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim("TicketId", ShowSeatId.ToString()),
            new Claim("EventName", ShowId.ToString()),
            new Claim("UserId", userId.ToString()),
            //new Claim("Expiry", ticket.Expiry.ToString("o")) // ISO 8601 format
        };

            var token = new JwtSecurityToken(
                issuer: "your-api.com",
                audience: "your-app",
                claims: claims,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<object> VerifyTicket(string qrToken)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["qrsecret"]));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "your-api.com",
                    ValidAudience = "your-app",
                    ValidateLifetime = true,
                    IssuerSigningKey = securityKey
                };

                var principal = tokenHandler.ValidateToken(qrToken, validationParameters, out _);
                var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);

                var ticket = await _context.ShowSeats
                    .Where(s => s.ShowSeatId == long.Parse(claims["TicketId"]) && s.UserId == long.Parse(claims["UserId"]))
                    .FirstOrDefaultAsync();

                if (ticket == null)
                {
                    return new { message = "Invalid or Expired Ticket" };
                }

                ticket.IsTokenUsed = true;
                ticket.TokenUsedTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new
                {
                    message = "Valid Ticket",
                    ticketId = ticket.StandSeatId,
                    stand = ticket.Stand.Name,
                    venue = ticket.Venue.Name,
                    show = ticket.Show.Name,
                };
            }
            catch (Exception)
            {
                return new { message = "Invalid or Expired Ticket" };
            }
        }

        internal async Task<List<BookedSeatDTO>> GetUserBookedShowSeatAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Getting booked seats for user with ID: {UserId}", userId);
                if (string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
                }
                var bookedSeats = await _context.ShowSeats
                    .Where(s => s.UserId == long.Parse(userId) && s.IsBooked == true)
                    .Include(s => s.Stand)
                    .Include(s => s.Venue)
                    .Include(s => s.Show)
                    .Include(s => s.StandSeat)
                    .Select(s => new BookedSeatDTO
                    {
                        ShowSeatId = s.ShowSeatId,
                        ShowId = s.ShowId,
                        ShowName = s.Show.Name,
                        Venuename = s.Venue.Name,
                        StandName = s.Stand.Name,
                        StandSeatId = s.StandSeatId,
                        ShowDate = s.Show.Date.ToString("yyyy-MM-dd"),
                        ShowTime = s.Show.Date.ToString("hh:mm tt"),
                        Token = s.Token,
                        IsTokenUsed = s.IsTokenUsed,
                        TokenUsedTime = s.TokenUsedTime,
                        BookingTime = s.BookingTime,
                        IsBooked = s.IsBooked
                    })
                    .ToListAsync();

                return bookedSeats;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving booked seats: {ex.Message}");
            }
        }
    }
}
