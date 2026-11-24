using HotelBooking.Data.Interfaces;

namespace HotelBooking.API.Services
{
    public class ApiCurrentUser : ICurrentUser
    {
        public string? Name => "api";
    }
}
