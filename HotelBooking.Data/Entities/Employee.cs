using HotelBooking.Data.Base;
using HotelBooking.Data.Enums;

namespace HotelBooking.Data.Entities
{
    public sealed class Employee : AuditEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public EmployeeRole Role { get; set; }
   
    }
}
