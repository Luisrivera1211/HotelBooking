using HotelBooking.Data.Enums;

namespace HotelBooking.Business.DTOs.Employee
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public EmployeeRole Role { get; set; }
    }
}
