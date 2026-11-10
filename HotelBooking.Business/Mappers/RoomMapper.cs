using HotelBooking.Business.DTOs.Room;
using HotelBooking.Data.Entities;

namespace HotelBooking.Business.Mappers
{
    public static class RoomMapper
    {
        public static RoomDto ToDto(Room room, RoomType type) => new()
        {
            Id = room.Id,
            FloorId = room.FloorId,
            RoomTypeId = room.RoomTypeId,
            Number = room.Number,
            Status = room.Status,
            RoomTypeName = type.Name,
            Capacity = type.Capacity,
            RatePerNight = type.RatePerNight
        };
    }
}
