using HotelBooking.Data.Entities;

namespace HotelBooking.Data.Interfaces
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task<List<Room>> GetByFloorAsync(int floorId);
        Task<bool> ExistsNumberInHotelAsync(int hotelId, string number);
        Task<List<Room>> GetAvailableAsync(int hotelId, DateTime checkIn, DateTime checkOut, int guests);

    }
}
