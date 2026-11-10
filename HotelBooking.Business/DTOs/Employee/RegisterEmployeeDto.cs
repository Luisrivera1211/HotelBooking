using HotelBooking.Data.Enums;

namespace HotelBooking.Business.DTOs.Employee
{
    public class RegisterEmployeeDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public EmployeeRole Role { get; set; } = EmployeeRole.Receptionist;

        // Solo se exige cuando Role es Administrator
        public string? AdminCode { get; set; }
    }
}
