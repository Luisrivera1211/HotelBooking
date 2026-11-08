using HotelBooking.Data.Entities;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Interfaces
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task<OperationResult<List<Room>>> GetByFloorAsync(int floorId);
        Task<OperationResult<bool>> ExistsNumberInHotelAsync(int hotelId, string number,  int? excludeRoomId = null);
        Task<OperationResult<List<Room>>> GetAvailableAsync(int hotelId, DateTime checkIn, DateTime checkOut, int guests);

    }
}
