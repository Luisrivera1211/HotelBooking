using HotelBooking.Data.Enums;

namespace HotelBooking.Web.Constants
{
    public static class Roles
    {
        public const string Administrator = nameof(EmployeeRole.Administrator);
        public const string Receptionist = nameof(EmployeeRole.Receptionist);
        public const string Staff = Administrator + "," + Receptionist;
    }
}
