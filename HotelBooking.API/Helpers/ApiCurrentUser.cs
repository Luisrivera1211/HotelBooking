using HotelBooking.Data.Interfaces;

namespace HotelBooking.API.Helpers
{
    public class ApiCurrentUser : ICurrentUser
    {
        public string? Name => "api";
    }
}
