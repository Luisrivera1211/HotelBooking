using HotelBooking.Data.Base;

namespace HotelBooking.Data.Entities
{
    public sealed class Guest : AuditEntity
    {
        public string FullName { get; set; } = string.Empty;

        public string Document { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public List<Reservation> Reservations { get; set; } = new();

        public List<Notification> Notifications { get; set; } = new();

    }
}
