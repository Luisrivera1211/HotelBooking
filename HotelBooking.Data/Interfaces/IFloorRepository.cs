using HotelBooking.Data.Entities;

namespace HotelBooking.Data.Interfaces
{
    public interface IFloorRepository : IBaseRepository<Floor>
    {
        Task<List<Floor>> GetByBlockIdAsync(int hotelId);
        Task<List<Floor>> GetActiveByBlockAsync(int blockId);
        Task<bool> ExistsActiveNumberAsync(int blockId, int number, int? excludeFloorId = null);
    }
}
