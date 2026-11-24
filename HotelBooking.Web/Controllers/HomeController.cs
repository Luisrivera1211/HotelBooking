using System.Diagnostics;
using HotelBooking.Business.DTOs.Reservation;
using HotelBooking.Business.Interfaces;
using HotelBooking.Data.Enums;
using HotelBooking.Web.Helpers;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    public class HomeController : AppController
    {
        private readonly IHotelService _hotelService;
        private readonly IGuestService _guestService;
        private readonly IReservationService _reservationService;

        public HomeController(IHotelService hotelService, IGuestService guestService, IReservationService reservationService)
        {
            _hotelService = hotelService;
            _guestService = guestService;
            _reservationService = reservationService;
        }

        // Pública: el visitante y el huésped ven la bienvenida; el personal ve el panel (como antes).
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated != true || User.IsGuest())
                return View("Welcome");

            var model = new DashboardViewModel();

            var hotels = await _hotelService.GetAllAsync();
            if (hotels.Success)
                model.ActiveHotels = hotels.Data!.Count(h => h.IsActive);

            var guests = await _guestService.GetAllAsync();
            if (guests.Success)
            {
                model.Guests = guests.Data!.Count;

                var all = new List<ReservationDto>();
                foreach (var g in guests.Data!)
                {
                    var r = await _reservationService.GetByGuestAsync(g.Id);
                    if (r.Success) all.AddRange(r.Data!);
                }

                var today = DateTime.Today;
                var active = all.Where(r => r.Status != ReservationStatus.Cancelled && r.CheckOut.Date >= today).ToList();
                model.UpcomingCount = active.Count;
                model.PendingCount = active.Count(r => r.Status == ReservationStatus.Pending);
                model.Upcoming = active.OrderBy(r => r.CheckIn).Take(5).ToList();
            }

            return View(model);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
