using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Guest;

namespace HotelBooking.Web.Models
{
    public class GuestFormViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El documento es obligatorio.")]
        [StringLength(50, ErrorMessage = "El documento no puede superar 50 caracteres.")]
        public string Document { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede superar 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(50, ErrorMessage = "El teléfono no puede superar 50 caracteres.")]
        public string Phone { get; set; } = string.Empty;

        public CreateGuestDto ToDto() => new()
        {
                FullName = FullName,
                Document = Document,
                Email = Email,
                Phone = Phone,
        };
    }
}
