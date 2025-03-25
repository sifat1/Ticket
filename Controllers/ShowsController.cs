

using App.Services;
using Dtos;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("get-ticket-reserve-pay")]
        public async Task<IActionResult> ReserveSeatsandPay([FromBody] List<ShowTicketPriceDTO> tickets)
        {
            try
            {
                object result = await _showService.ReserveSeats(tickets);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
