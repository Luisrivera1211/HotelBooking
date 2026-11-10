using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.RoomType;

namespace HotelBooking.Web.Models
{
    // Modelo del formulario: la validación de entrada vive en la capa de presentación;
    // Business recibe un DTO sin anotaciones y vuelve a validar por su cuenta.
    public class RoomTypeFormViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor que cero.")]
        public int Capacity { get; set; }

        [Range(0, 99999999.99, ErrorMessage = "La tarifa por noche no puede ser negativa.")]
        public decimal RatePerNight { get; set; }

        public CreateRoomTypeDto ToDto() => new()
        {
            Name = Name,
            Capacity = Capacity,
            RatePerNight = RatePerNight,
        };
    }
}
