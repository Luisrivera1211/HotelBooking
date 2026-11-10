using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Availability;

namespace HotelBooking.Web.Models
{
    // Modelo del formulario: la validación de entrada vive en la capa de presentación;
    // Business recibe un DTO sin anotaciones y vuelve a validar por su cuenta.
    public class AvailabilitySearchViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un hotel.")]
        public int HotelId { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de huéspedes debe ser mayor que cero.")]
        public int GuestCount { get; set; }

        public AvailabilitySearchDto ToDto() => new()
        {
            HotelId = HotelId,
            CheckIn = CheckIn,
            CheckOut = CheckOut,
            GuestCount = GuestCount,
        };
    }
}
