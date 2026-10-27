using HotelBooking.Data.Base;
using HotelBooking.Data.Context;

namespace HotelBooking.Data.Entities
{
    public sealed class RoomType : AuditEntity
    {

        public string Name { get; set; }
        public int Capacity { get; set; }
        public decimal RatePerNight { get; set; }

        public List<Room> Rooms { get; set; } = new();

    }
}
