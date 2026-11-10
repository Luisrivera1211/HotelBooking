using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class BlockRepository : BaseRepository<Block>, IBlockRepository
    {
        public BlockRepository(HotelBookingDbContext context) : base(context)
        { }

        protected override string? Validate(Block block)
        {
            if (string.IsNullOrWhiteSpace(block.Name))
                return DataMessages.Required("Nombre");
            if (block.Name.Length > 50)
                return DataMessages.TooLong("Nombre", 50);
            return null;
        }

        public async Task<OperationResult<List<Block>>> GetByHotelIdAsync(int hotelId)
        {
            try
            {
                if (hotelId <= 0)
                    return OperationResult<List<Block>>.Validation(DataMessages.InvalidId);

                var blocks = await _context.Blocks
                    .AsNoTracking()
                    .Where(b => b.HotelId == hotelId)
                    .ToListAsync();
                return OperationResult<List<Block>>.Ok(blocks);
            }
            catch (Exception)
            {
                return OperationResult<List<Block>>.Unexpected(DataMessages.QueryError);
            }
        }

        public async Task<OperationResult<bool>> ExistsNameInHotelAsync(int hotelId, string name, int? excludeBlockId = null)
        {
            try
            {
                if (hotelId <= 0)
                    return OperationResult<bool>.Validation(DataMessages.InvalidId);
                if (string.IsNullOrWhiteSpace(name))
                    return OperationResult<bool>.Validation(DataMessages.Required("Nombre"));

                var query = _context.Blocks.Where(b => b.HotelId == hotelId && b.Name == name.Trim());

                if (excludeBlockId.HasValue)
                    query = query.Where(b => b.Id != excludeBlockId.Value);

                return OperationResult<bool>.Ok(await query.AnyAsync());
            }
            catch (Exception)
            {
                return OperationResult<bool>.Unexpected(DataMessages.QueryError);
            }
        }
    }
}
