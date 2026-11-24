using HotelBooking.Data.Interfaces;

namespace HotelBooking.Web.Helpers
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string? Name => _accessor.HttpContext?.User?.Identity?.Name;
    }
}