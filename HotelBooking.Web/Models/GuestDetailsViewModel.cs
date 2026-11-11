using HotelBooking.Business.DTOs.Guest;
using HotelBooking.Business.DTOs.Reservation;

namespace HotelBooking.Web.Models
{
    public class GuestDetailsViewModel
    {
        public GuestDto Guest { get; set; } = new();
        public List<ReservationDto> Reservations { get; set; } = new();
    }
}
