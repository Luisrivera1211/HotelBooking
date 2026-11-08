using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class FloorRepository : BaseRepository<Floor>, IFloorRepository
    {
        public FloorRepository(HotelBookingDbContext context) : base(context)
        { }

        protected override string? Validate(Floor floor)
        {
            if (floor.Number <= 0)
                return DataMessages.MustBePositive("Número");
            return null;
        }

        public async Task<OperationResult<List<Floor>>> GetByBlockIdAsync(int blockId)
        {
            try
            {
                if (blockId <= 0)
                    return OperationResult<List<Floor>>.Validation(DataMessages.InvalidId);

                var floors = await _context.Floors
                    .AsNoTracking()
                    .Where(f => f.BlockId == blockId)
                    .ToListAsync();
                return OperationResult<List<Floor>>.Ok(floors);
            }
            catch (Exception)
            {
                return OperationResult<List<Floor>>.Unexpected(DataMessages.QueryError);
            }
        }
            
        public async Task<OperationResult<List<Floor>>> GetActiveByBlockAsync(int blockId)
        {
            try
            {
                if (blockId <= 0)
                    return OperationResult<List<Floor>>.Validation(DataMessages.InvalidId);

                var floors = await _context.Floors
                  .AsNoTracking()
                  .Where(f => f.BlockId == blockId && f.IsActive)
                  .ToListAsync();
                return OperationResult<List<Floor>>.Ok(floors);
            }
            catch (Exception)
            {
                return OperationResult<List<Floor>>.Unexpected (DataMessages.QueryError);
            }
        }

        public async Task<OperationResult<bool>> ExistsActiveNumberAsync(int blockId, int number, int? excludeFloorId = null)
        {
            try
            {
               if (blockId <= 0)
                    return OperationResult<bool>.Validation(DataMessages.InvalidId);
                if (number <= 0)
                    return OperationResult<bool>.Validation(DataMessages.MustBePositive("Numero"));

                var query = _context.Floors.Where(f => f.BlockId == blockId && f.Number == number && f.IsActive);

                if (excludeFloorId.HasValue)
                    query = query.Where(f => f.Id != excludeFloorId.Value);

                return OperationResult<bool>.Ok(await query.AnyAsync());
            }
            catch (Exception)
            {
                return OperationResult<bool>.Unexpected(DataMessages.QueryError);
            }

        }
    }

}

