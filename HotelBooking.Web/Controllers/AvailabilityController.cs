using HotelBooking.Business.DTOs.Availability;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [Authorize(Roles = Roles.Staff)]
    public class AvailabilityController : AppController
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly IHotelService _hotelService;

        public AvailabilityController(IAvailabilityService availabilityService, IHotelService hotelService)
        {
            _availabilityService = availabilityService;
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var search = new AvailabilitySearchViewModel
            {
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(1),
                GuestCount = 1
            };
            return await BuildView(search, rooms: null);
        }

        // Se enlaza SOLO el DTO del formulario (el resto del view model tiene propiedades no anulables
        // que el binder trataría como obligatorias)
        [HttpPost]
        public async Task<IActionResult> Index([Bind(Prefix = nameof(AvailabilityViewModel.Search))] AvailabilitySearchViewModel search)
        {
            if (!ModelState.IsValid)
                return await BuildView(search, rooms: null);

            var result = await _availabilityService.SearchAsync(search.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return await BuildView(search, rooms: null);
            }

            return await BuildView(search, result.Data);
        }

        private async Task<IActionResult> BuildView(AvailabilitySearchViewModel search, List<HotelBooking.Business.DTOs.Room.RoomDto>? rooms)
        {
            var hotels = await _hotelService.GetAllAsync();
            if (!hotels.Success)
                ShowError(hotels.Message);

            ViewData["Title"] = "Disponibilidad";
            return View(new AvailabilityViewModel
            {
                Search = search,
                // Solo hoteles activos para buscar
                Hotels = hotels.Success ? hotels.Data!.Where(h => h.IsActive).ToList() : new(),
                Rooms = rooms
            });
        }
    }
}
