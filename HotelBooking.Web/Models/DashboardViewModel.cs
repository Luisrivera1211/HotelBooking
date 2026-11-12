using HotelBooking.Business.DTOs.Reservation;

namespace HotelBooking.Web.Models
{
    public class DashboardViewModel
    {
        public int ActiveHotels { get; set; }
        public int Guests { get; set; }
        public int UpcomingCount { get; set; }
        public int PendingCount { get; set; }
        public List<ReservationDto> Upcoming { get; set; } = new();
    }
}
