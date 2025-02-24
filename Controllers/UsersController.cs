using DB.DBcontext;
using Dtos;
using JWTAuthServer.DTOs;
using Microsoft.AspNetCore.Mvc;
using User.Registration;

namespace App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRegistrationService _userManager;
        private readonly IConfiguration _configuration;
        private readonly ShowDbContext _context;
        public UsersController(IConfiguration configuration, UserRegistrationService userRegistrationService,ShowDbContext context)
        {
            _userManager = userRegistrationService;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("registration")]
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
        
        [HttpGet("login")]
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

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshDTO model)
        {
            await _userManager.Refresh(model);
            return Ok("");
        }

        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout(RefreshDTO refreshDto)
        {
            try{
                await _userManager.Logout(refreshDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Login");
        }

    }
}