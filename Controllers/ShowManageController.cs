
using App.Services;
using Dtos;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowManageController : ControllerBase
    {
        private readonly ShowService _showService;

        public ShowManageController(ShowService showService)
        {
            _showService = showService;
        }

        [HttpPost("add-show")]
        public async void AddShowAsync(CreateShow show)
        {
            AddShowAsync(show);
        }
    }
}