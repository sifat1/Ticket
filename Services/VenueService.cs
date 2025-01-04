using DB.DBcontext;
using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels;

namespace App.services{
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

    public async Task AddVenueAsync(Venue venue)
    {
        _context.Venues.Add(venue);
        await _context.SaveChangesAsync();
    }
}
}