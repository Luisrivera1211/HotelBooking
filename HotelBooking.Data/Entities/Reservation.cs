using HotelBooking.Data.Base;
using HotelBooking.Data.Enums;

namespace HotelBooking.Data.Entities
{
    public sealed class Reservation : AuditEntity
    {

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public int GuestCount { get; set; }

        public decimal Total { get; set; }

        public int GuestId { get; set; }

        public int RoomId { get; set; }
    
        public ReservationStatus Status { get; set; }

        public Room Room { get; set; } = null!;
        public Guest Guest { get; set; } = null!;
        public List<Notification> Notifications { get; set; } = new();

    }
}
