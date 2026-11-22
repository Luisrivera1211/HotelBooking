using System.Security.Claims;
using HotelBooking.Web.Constants;

namespace HotelBooking.Web.Helpers
{
    public static class UserExtensions
    {
        public static bool IsGuest(this ClaimsPrincipal user) => user.IsInRole(Roles.Guest);

        public static int CurrentId(this ClaimsPrincipal user) =>
            int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    }
}
