using System.ComponentModel.DataAnnotations;
using HotelBooking.Business.DTOs.Employee;
using HotelBooking.Data.Enums;

namespace HotelBooking.Web.Models
{
    
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede superar 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme la contraseña.")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public EmployeeRole Role { get; set; } = EmployeeRole.Receptionist;

        [DataType(DataType.Password)]
        public string? AdminCode { get; set; } = null;

        public RegisterEmployeeDto ToDto() => new()
        {
                FullName = FullName,
                Email = Email,
                Password = Password,
                ConfirmPassword = ConfirmPassword,
                Role = Role,
                AdminCode = AdminCode,
        };
    }
}
