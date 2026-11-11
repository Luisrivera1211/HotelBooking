using HotelBooking.Business.DTOs.Notification;
using HotelBooking.Business.DTOs.Reservation;

namespace HotelBooking.Web.Models
{
    public class ReservationDetailsViewModel
    {
        public ReservationDto Reservation { get; set; } = new();
        public List<NotificationDto> Notifications { get; set; } = new();
    }
}
