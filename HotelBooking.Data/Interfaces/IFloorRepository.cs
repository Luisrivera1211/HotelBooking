using HotelBooking.Data.Entities;

namespace HotelBooking.Data.Interfaces
{
    public interface IFloorRepository : IBaseRepository<Floor>
    {
        Task<List<Floor>> GetByHotelIdAsync(int hotelId);
        Task<List<Floor>> GetActiveByHotelAsync(int hotelId);
        Task<bool> ExistsActiveNumberAsync(int hotelId, int number, int? excludeFloorId = null);
    }
}
