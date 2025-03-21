
using App.Services;
using App.Services.Manager;
using Dtos;
using Microsoft.AspNetCore.Mvc;
using ShowTickets.Ticketmodels;

namespace App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowManageController : ControllerBase
    {
        private readonly ShowManagerService _showService;
        private readonly VenueService _venueService;

        public ShowManageController(ShowManagerService showService, VenueService venueService)
        {
            _showService = showService;
            _venueService = venueService;
        }

        [HttpPost("add-show")]
        public async void AddShow(CreateShow show)
        {
            await _showService.AddShowTask(show);
        }

        [HttpPost("add-stand")]
        public async void AddStandSeatonStand(CreateStand stand)
        {
            await _showService.AddStandAsync(stand);
        }

        [HttpPost("add-venue")]
        public void AddNewVenue(CreateVenue createVenue)
        {
            _venueService.AddVenue(createVenue);
        }

        [HttpPost("add-show-opening")]
        public async Task<IActionResult> AddShowOpening(ShowOpening showOpening)
        {
            try{
                await _showService.SetTicketOpeningAsync(showOpening);
                return Ok("Show opening added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("add-show-seat-price")]
        public async Task<IActionResult> AddShowSeatPrice(ShowTicketPriceDTO showSeatPrice)
        {
            try
            {
                await _showService.AddShowSeatPriceAsync(showSeatPrice);
                return Ok("Show seat price added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
    }
}