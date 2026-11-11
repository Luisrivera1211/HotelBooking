using HotelBooking.Data.Enums;

namespace HotelBooking.Web.Constants
{
    // Los valores deben coincidir con EmployeeRole.ToString() (se guardan como claims)
    public static class Roles
    {
        public const string Administrator = nameof(EmployeeRole.Administrator);
        public const string Receptionist = nameof(EmployeeRole.Receptionist);

        // El administrador puede todo lo que hace el recepcionista
        public const string Staff = Administrator + "," + Receptionist;
    }
}
