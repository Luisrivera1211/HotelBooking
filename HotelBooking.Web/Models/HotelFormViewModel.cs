using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Hotels;

namespace HotelBooking.Web.Models
{
    public class HotelFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(100, ErrorMessage = "La dirección no puede superar 100 caracteres.")]
        public string Address { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Las estrellas deben estar entre 1 y 5.")]
        public int Stars { get; set; } = 3;

        public CreateHotelDto ToDto() => new()
        {
            Name = Name,
            Address = Address,
            Stars = Stars,
        };
    }
}
