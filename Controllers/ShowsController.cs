

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ShowService _showService;
        private readonly ILogger<ShowsController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShowsController(ShowService showService,
         ILogger<ShowsController> logger,
         IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _showService = showService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetShows()
        {
            var shows = await _showService.GetShowsAsync();
            return Ok(shows);
        }

        [HttpGet("get-seats/{showid}/{standid}")]
        public async Task<IActionResult> GetSeats(int showid, int standid)
        {
            var seats = await _showService.GetSeatsAsync(showid, standid);
            return Ok(seats);
        }

        //[Authorize(Roles = "User")]
        [HttpGet("get-Shows-with-sellingdate")]
        public async Task<IActionResult> GetShowsOpening()
        {
            try
            {

                return Ok(await _showService.GetTicketOpeningAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-Shows-venue-stands/{venueId}")]
        public async Task<IActionResult> GetVenueStands(int venueId)
        {
            try
            {
                return Ok(await _showService.getstands(venueId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-Shows-stands/{ShowId}")]
        public async Task<IActionResult> GetShowStand(int ShowId)
        {
            try
            {
                return Ok(await _showService.GetShowStand(ShowId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost("get-ticket-price")]
        public async Task<IActionResult> GetTicketPrice([FromBody] List<ShowTicketPriceDTO> tickets)
        {
            try
            {
                object result = await _showService.GetTicketPrice(tickets);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost("get-ticket-reserve-pay")]
        public async Task<IActionResult> ReserveSeatsandPay([FromBody] List<ShowTicketPriceDTO> tickets)
        {
            try
            {
                var claims = _httpContextAccessor.HttpContext?.User?.Claims;

                if (claims == null || !claims.Any())
                {
                    return Unauthorized(new { message = "No claims found in token" });
                }

                var userIdString = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    return Unauthorized(new { message = "User ID is invalid or not found in token", claims = claims.Select(c => new { c.Type, c.Value }) });
                }

                object result = await _showService.ReserveSeats(tickets, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-user-booked-show-seat")]
        public async Task<IActionResult> GetUserBookedShowSeat()
        {
            try
            {

                var claims = _httpContextAccessor.HttpContext?.User?.Claims;

                if (claims == null || !claims.Any())
                {
                    return Unauthorized(new { message = "No claims found in token" });
                }

                var userIdString = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    return Unauthorized(new { message = "User ID is invalid or not found in token", claims = claims.Select(c => new { c.Type, c.Value }) });
                }



                List<BookedSeatDTO> bookedSeats = await _showService.GetUserBookedShowSeatAsync(userId.ToString());
                _logger.LogInformation($"User {userId} booked seats retrieved successfully.");
                return Ok(new { data = bookedSeats });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }


}
