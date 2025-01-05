


using App.Services;
using Microsoft.AspNetCore.Mvc;
using ShowTickets.Ticketmodels;

namespace App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenuesController : ControllerBase
    {
        private readonly VenueService _venueService;

        public VenuesController(VenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVenues()
        {
            var venues = await _venueService.GetAllVenuesAsync();
            return Ok(venues);
        }

        [HttpPost]
        public async Task<IActionResult> AddVenue([FromBody] Venue venue)
        {
            await _venueService.AddVenueAsync(venue);
            return Ok();
        }
    }
}
