using HotelBooking.Data.Base;
using HotelBooking.Data.Enums;

namespace HotelBooking.Data.Entities
{
    public sealed class Notification : AuditEntity
    {
        public string Type { get; set; }

        public string Message { get; set; }

        public DateTime SentAt { get; set; }

        public RoomStatus Status { get; set; }

        public int ReservatioId { get; set; }

        public int GuestId { get; set; }
    }
}
