using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class BlockRepository : BaseRepository<Block>, IBlockRepository
    {
        public BlockRepository(HotelBookingDbContext context) : base(context)
        { }

        public async Task<List<Block>> GetBlockAsync()
        {
            return await _context.Blocks.ToListAsync();
        }

        public async Task<Block?> GetByNameAsync(string name)
        {
            return await _context.Blocks.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<bool> ExistsActiveNumbebrAsync(int blockId, int number, int? excludeFloorId = null)
        {
            var query = _context.N.Where(f =>
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
