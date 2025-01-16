using DB.DBcontext;
using Dtos;
using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels;

namespace App.Services
{
    public class VenueService
    {
        private readonly ShowDbContext _context;

        public VenueService(ShowDbContext context)
        {
            _context = context;
        }

        public async Task<List<Venue>> GetAllVenuesAsync()
        {
            return await _context.Venues.Include(v => v.Stands).ToListAsync();
        }

        public async void AddVenue(CreateVenue venue)
        {
            if (venue == null)
            {
                throw new ArgumentNullException(nameof(venue));
            }

            var newvenue = new Venue();
            newvenue.Name = venue.Name;
            newvenue.Location = venue.Location;
            _context.Venues.Add(newvenue);
            _context.SaveChanges();
        }
    }
}