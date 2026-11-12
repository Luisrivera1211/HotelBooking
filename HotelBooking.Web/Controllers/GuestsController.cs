using HotelBooking.Business.DTOs.Guest;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [Authorize(Roles = Roles.Staff)]
    public class GuestsController : AppController
    {
        private readonly IGuestService _guestService;
        private readonly IReservationService _reservationService;

        public GuestsController(IGuestService guestService, IReservationService reservationService)
        {
            _guestService = guestService;
            _reservationService = reservationService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _guestService.GetAllAsync();
            if (!result.Success)
            {
                ShowError(result.Message);
                return View(new List<GuestDto>());
            }

            return View(result.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var guest = await _guestService.GetByIdAsync(id);
            if (!guest.Success)
                return RedirectError(guest.Message, nameof(Index));

            var reservations = await _reservationService.GetByGuestAsync(id);
            if (!reservations.Success)
                ShowError(reservations.Message);

            return View(new GuestDetailsViewModel
            {
                Guest = guest.Data!,
                Reservations = reservations.Success ? reservations.Data! : new()
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return FormView(new GuestFormViewModel(), "Nuevo huésped", nameof(Create), null);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GuestFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return FormView(dto, "Nuevo huésped", nameof(Create), null);

            var result = await _guestService.CreateAsync(dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return FormView(dto, "Nuevo huésped", nameof(Create), null);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Details), new { id = result.Data!.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _guestService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectError(result.Message, nameof(Index));

            var g = result.Data!;
            var dto = new GuestFormViewModel { FullName = g.FullName, Document = g.Document, Email = g.Email, Phone = g.Phone };
            return FormView(dto, "Editar huésped", nameof(Edit), id);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, GuestFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return FormView(dto, "Editar huésped", nameof(Edit), id);

            var result = await _guestService.UpdateAsync(id, dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return FormView(dto, "Editar huésped", nameof(Edit), id);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Details), new { id });
        }

        private ViewResult FormView(GuestFormViewModel dto, string title, string action, int? id)
        {
            ViewData["Title"] = title;
            ViewData["FormAction"] = action;
            ViewData["Id"] = id;
            return View("Form", dto);
        }
    }
}
