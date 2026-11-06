using HotelBooking.Data.Entities;

namespace HotelBooking.Business.Interfaces
{
    public interface IRoomTypeService
    {
        Task<RoomType> CreateRoomTypeAsync(int RoomId);

        Task <RoomType> FindBty

    }
}
