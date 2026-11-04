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

        public async Task<List<Floor>> GetByBlockIdAsync(int blockId)
        {
            return await _context.Floors.Where(h => h.BlockId == blockId).ToListAsync();
        }
        public async Task<List<Floor>> GetActiveByBlockAsync(int blockId)
        {
            return await _context.Floors.Where(h => h.BlockId == blockId && h.IsActive == true).ToListAsync();
        }
        public async Task<bool> ExistsActiveNumberAsync(int blockId, int number, int? excludeFloorId = null)
        {
            var query = _context.Floors.Where(f =>
            f.BlockId == blockId
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

