using HotelBooking.Data.Base;
using HotelBooking.Data.Enums;

namespace HotelBooking.Data.Entities
{
    public sealed class Room : AuditEntity
    {
        public int FloorId { get; set; }

        public int RoomTypeId { get; set; }

        public string Number { get; set; }

        public RoomStatus Status { get; set; }

        public Floor Floor { get; set; } = null!;

        public RoomType RoomType { get; set; } = null!;

        public List<Reservation> Reservations { get; set; } = new();

    }
}
