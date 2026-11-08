using HotelBooking.Data.Base;
using HotelBooking.Data.Enums;

namespace HotelBooking.Data.Entities
{
    public class Employee : AuditEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public EmployeeRole Role { get; set; }
   
    }
}
