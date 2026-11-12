using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Reservation;

namespace HotelBooking.Web.Models
{

    public class ReservationEditViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una habitación.")]
        public int RoomId { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de huéspedes debe ser mayor que cero.")]
        public int GuestCount { get; set; }

        public ModifyReservationDto ToDto() => new()
        {
                RoomId = RoomId,
                CheckIn = CheckIn,
                CheckOut = CheckOut,
                GuestCount = GuestCount,
        };
    }
}
