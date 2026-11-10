using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Hotels;

namespace HotelBooking.Web.Models
{
    // Modelo del formulario: la validación de entrada vive en la capa de presentación;
    // Business recibe un DTO sin anotaciones y vuelve a validar por su cuenta.
    public class HotelFormViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(100, ErrorMessage = "La dirección no puede superar 100 caracteres.")]
        public string Address { get; set; } = string.Empty;

        public CreateHotelDto ToDto() => new()
        {
            Name = Name,
            Address = Address,
        };
    }
}
