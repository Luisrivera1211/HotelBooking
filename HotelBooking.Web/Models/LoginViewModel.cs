using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Employee;

namespace HotelBooking.Web.Models
{
    // Modelo del formulario: la validación de entrada vive en la capa de presentación;
    // Business recibe un DTO sin anotaciones y vuelve a validar por su cuenta.
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public LoginDto ToDto() => new()
        {
            Email = Email,
            Password = Password,
        };
    }
}
