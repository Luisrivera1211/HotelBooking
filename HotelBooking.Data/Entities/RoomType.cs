using HotelBooking.Data.Base;

namespace HotelBooking.Data.Entities
{
    public class RoomType : AuditEntity
    {
        public int Capacity { get; set; }

        public decimal RatePerNight { get; set; }

    }
}
