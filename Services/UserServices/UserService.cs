
using DB.DBcontext;
using Dtos;
using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels.User;

namespace User.Registration
{
    public class UserRegistrationService
    {
        private readonly ShowDbContext _context;
        public UserRegistrationService(ShowDbContext dbContext)
        {
            _context = dbContext;
        }

        public string Createuser(RegistrationDTO registrationDTO)
        {
            if (registrationDTO == null)
            {
                throw new ArgumentNullException(nameof(registrationDTO));
            }

            var user = _context.Users.Where(e => e.Email == registrationDTO.Email).FirstOrDefault();

            if (user != null)
            {
                return "user already exists";
            }

            var newuser = new Users();
            newuser.Email = registrationDTO.Email;
            newuser.Name = registrationDTO.Name;
            newuser.PhoneNumber = registrationDTO.Phone;

            _context.Add(newuser);
            _context.SaveChanges();

            return "";


        }
    }
}