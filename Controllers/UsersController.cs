using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using DB.DBcontext;
using Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShowTickets.Ticketmodels.User;
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