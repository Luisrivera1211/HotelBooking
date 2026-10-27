using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class HotelRepository : BaseRepository<Hotel>, IHotelRepository
    {
        public HotelRepository(HotelBookingDbContext context) : base(context)
        { }

        public async Task<List<Hotel>> GetActiveHotelsAsync()
        {
            return await _context.Hotels.Where(h => h.IsActive).ToListAsync();
        }

        public async Task<Hotel?> GetByNameAsync(string name)
        {
            return await _context.Hotels.FirstOrDefaultAsync(h => h.Name == name);
        }
    }
}
