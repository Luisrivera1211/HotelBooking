using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class FloorRepository : BaseRepository<Floor>, IFloorRepository
    {
        public FloorRepository(HotelBookingDbContext context) : base(context)
        { }

        public async Task<List<Floor>> GetByHotelIdAsync(int hotelId)
        {
            return await _context.Floors.Where(h => h.HotelId == hotelId).ToListAsync();
        }
        public async Task<List<Floor>> GetActiveByHotelAsync(int hotelId)
        {
            return await _context.Floors.Where(h => h.HotelId == hotelId && h.IsActive == true).ToListAsync();
        }
        public async Task<bool> ExistsActiveNumberAsync(int hotelId, int number, int? excludeFloorId = null)
        {
            var query = _context.Floors.Where(f =>
            f.HotelId == hotelId
            && f.Number == number
            && f.IsActive);

           if (excludeFloorId.HasValue)
            {
                query = query.Where(f => f.Id != excludeFloorId.Value);
            }

            return await query.AnyAsync();
        }
    }

}

