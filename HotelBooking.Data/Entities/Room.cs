using HotelBooking.Data.Base;
using HotelBooking.Data.Enums;

namespace HotelBooking.Data.Entities
{
    public sealed class Room : AuditEntity
    {
        public int FloorId { get; set; }

        public int RoomTypeId { get; set; }

        public RoomStatus Status { get; set; }
    }
}
