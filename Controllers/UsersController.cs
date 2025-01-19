using Dtos;
using Microsoft.AspNetCore.Mvc;
using User.Registration;

namespace App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRegistrationService _userManager;
        public UsersController(UserRegistrationService userRegistrationService)
        {
            _userManager = userRegistrationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegistrationDTO registrationDTO)
        {
            try
            {
                await _userManager.Createuser(registrationDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("User Created successfully");
        }
        
        [HttpGet]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                await _userManager.Login(loginDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Login Successfull");
        }
    }
}