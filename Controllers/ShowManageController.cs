
using App.Services;
using Dtos;
using Microsoft.AspNetCore.Mvc;
using ShowTickets.Ticketmodels;

namespace App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowManageController : ControllerBase
    {
        private readonly ShowService _showService;
        private readonly VenueService _venueService;

        public ShowManageController(ShowService showService, VenueService venueService)
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

        
    }
}