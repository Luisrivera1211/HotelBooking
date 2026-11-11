using HotelBooking.Business.DTOs.Reservation;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [Authorize(Roles = Roles.Staff)]
    public class ReservationsController : AppController
    {
        private readonly IReservationService _reservationService;
        private readonly IGuestService _guestService;
        private readonly IRoomService _roomService;
        private readonly INotificationService _notificationService;

        public ReservationsController(
            IReservationService reservationService,
            IGuestService guestService,
            IRoomService roomService,
            INotificationService notificationService)
        {
            _reservationService = reservationService;
            _guestService = guestService;
            _roomService = roomService;
            _notificationService = notificationService;
        }

        // Llega desde la pantalla de disponibilidad con la habitación y fechas ya elegidas
        [HttpGet]
        public async Task<IActionResult> Create(int roomId, DateTime checkIn, DateTime checkOut, int guestCount)
        {
            var room = await _roomService.GetByIdAsync(roomId);
            if (!room.Success)
                return RedirectError(room.Message, "Index", "Availability");

            var model = new NewReservationViewModel
            {
                RoomId = roomId,
                RoomLabel = RoomLabel(room.Data!),
                CheckIn = checkIn,
                CheckOut = checkOut,
                GuestCount = guestCount < 1 ? 1 : guestCount
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewReservationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // El recepcionista identifica al huésped por documento
            var guest = await _guestService.GetByDocumentAsync(model.Document.Trim());
            if (!guest.Success)
            {
                AddError(guest);
                return View(model);
            }

            var result = await _reservationService.CreateAsync(new CreateReservationDto
            {
                RoomId = model.RoomId,
                GuestId = guest.Data!.Id,
                CheckIn = model.CheckIn,
                CheckOut = model.CheckOut,
                GuestCount = model.GuestCount
            });

            if (!result.Success)
            {
                AddError(result);
                return View(model);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Details), new { id = result.Data!.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (!reservation.Success)
                return RedirectError(reservation.Message, "Index", "Guests");

            var notifications = await _notificationService.GetByReservationAsync(id);
            if (!notifications.Success)
                ShowError(notifications.Message);

            return View(new ReservationDetailsViewModel
            {
                Reservation = reservation.Data!,
                Notifications = notifications.Success ? notifications.Data! : new()
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _reservationService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectError(result.Message, "Index", "Guests");

            var r = result.Data!;
            var dto = new ReservationEditViewModel
            {
                RoomId = r.RoomId,
                CheckIn = r.CheckIn,
                CheckOut = r.CheckOut,
                GuestCount = r.GuestCount
            };
            return EditView(dto, id);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ReservationEditViewModel dto)
        {
            if (!ModelState.IsValid)
                return EditView(dto, id);

            var result = await _reservationService.ModifyAsync(id, dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return EditView(dto, id);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _reservationService.ConfirmAsync(id);
            return FlashAndRedirect(result, result.Message, nameof(Details), routeValues: new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _reservationService.CancelAsync(id);
            return FlashAndRedirect(result, result.Message, nameof(Details), routeValues: new { id });
        }

        private ViewResult EditView(ReservationEditViewModel dto, int id)
        {
            ViewData["Title"] = $"Modificar reserva #{id}";
            ViewData["Id"] = id;
            return View("Edit", dto);
        }

        private static string RoomLabel(HotelBooking.Business.DTOs.Room.RoomDto r) =>
            $"Habitación {r.Number} · {r.RoomTypeName} · {r.RatePerNight:C}/noche";
    }
}
