using HotelBooking.Data.Entities;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Interfaces
{
    public interface IFloorRepository : IBaseRepository<Floor>
    {
        Task<OperationResult<List<Floor>>> GetByBlockIdAsync(int blockId);
        Task<OperationResult<bool>> ExistsActiveNumberAsync(int blockId, int number, int? excludeFloorId = null);
    }
}
