

using App.Services;
using Dtos;
using Microsoft.AspNetCore.Mvc;
using ShowTickets.Ticketmodels;

namespace App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ShowService _showService;

        public ShowsController(ShowService showService)
        {
            _showService = showService;
        }

        [HttpGet]
        public async Task<IActionResult> GetShows()
        {
            var shows = await _showService.GetShowsAsync();
            return Ok(shows);
        }

        [HttpGet("{id}/available-seats")]
        public async Task<IActionResult> GetAvailableSeats(int id)
        {
            var seats = await _showService.GetAvailableSeatsAsync(id);
            return Ok(seats);
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookTicket([FromBody] BookingRequest request)
        {
            var result = await _showService.BookTicketAsync(request);
            return Ok(new { Message = result });
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelTicket([FromBody] BookingRequest request)
        {
            var result = await _showService.CancelBookingAsync(request);
            return Ok(new { Message = result });
        }

        [HttpPost("book-with-concurrency")]
        public async Task<IActionResult> BookTicketWithConcurrency([FromBody] BookingRequest request)
        {
            var result = await _showService.BookTicketWithOptimisticConcurrencyAsync(request);
            return Ok(new { Message = result });
        }

        [HttpGet("get-Shows-with-sellingdate")]
        public async Task<IActionResult> GetShowsOpening()
        {
            try
            {
            return Ok (await _showService.GetTicketOpeningAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
