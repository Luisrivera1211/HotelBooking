using HotelBooking.Data.Enums;

namespace HotelBooking.Business.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
        public NotificationStatus Status { get; set; }
        public int ReservationId { get; set; }
        public int GuestCount { get; set; }
        public string Type { get; set; }

    }
}
