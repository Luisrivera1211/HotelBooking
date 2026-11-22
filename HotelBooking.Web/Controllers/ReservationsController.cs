using HotelBooking.Business.DTOs.Reservation;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Helpers;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [Authorize]
    public class ReservationsController : AppController
    {
        private readonly IReservationService _reservationService;
        private readonly IGuestService _guestService;
        private readonly IRoomService _roomService;
        private readonly INotificationService _notificationService;
        private readonly IAuditService _auditService;

        public ReservationsController(
            IReservationService reservationService,
            IGuestService guestService,
            IRoomService roomService,
            INotificationService notificationService,
            IAuditService auditService)
        {
            _reservationService = reservationService;
            _guestService = guestService;
            _roomService = roomService;
            _notificationService = notificationService;
            _auditService = auditService;
        }

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
            return await CreateView(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewReservationViewModel model)
        {
            if (User.IsGuest())
            {
                var me = await _guestService.GetByIdAsync(User.CurrentId());
                if (!me.Success)
                    return RedirectError(me.Message, "Index", "Availability");

                model.Document = me.Data!.Document;
                ModelState.Remove(nameof(model.Document));
            }

            if (!ModelState.IsValid)
                return await CreateView(model);

            var guest = await _guestService.GetByDocumentAsync(model.Document.Trim());
            if (!guest.Success)
            {
                AddError(guest);
                return await CreateView(model);
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
                return await CreateView(model);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Details), new { id = result.Data!.Id });
        }

        [Authorize(Roles = Roles.Guest)]
        public async Task<IActionResult> Mine()
        {
            var result = await _reservationService.GetByGuestAsync(User.CurrentId());
            if (!result.Success)
                ShowError(result.Message);

            return View(result.Success ? result.Data! : new List<ReservationDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (!reservation.Success)
                return User.IsGuest()
                    ? RedirectError(reservation.Message, nameof(Mine))
                    : RedirectError(reservation.Message, "Index", "Guests");

            if (User.IsGuest() && reservation.Data!.GuestId != User.CurrentId())
                return NotFound();

            var notifications = await _notificationService.GetByReservationAsync(id);
            if (!notifications.Success)
                ShowError(notifications.Message);

            var history = await _auditService.GetByEntityAsync("Reservation", id);
            if (!history.Success)
                ShowError(history.Message);

            return View(new ReservationDetailsViewModel
            {
                Reservation = reservation.Data!,
                Notifications = notifications.Success ? notifications.Data! : new(),
                History = history.Success ? history.Data! : new()
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await CanAccessAsync(id))
                return NotFound();

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
            if (!await CanAccessAsync(id))
                return NotFound();

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
            if (!await CanAccessAsync(id))
                return NotFound();

            var result = await _reservationService.ConfirmAsync(id);
            return FlashAndRedirect(result, result.Message, nameof(Details), routeValues: new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!await CanAccessAsync(id))
                return NotFound();

            var result = await _reservationService.CancelAsync(id);
            return FlashAndRedirect(result, result.Message, nameof(Details), routeValues: new { id });
        }

        private async Task<bool> CanAccessAsync(int reservationId)
        {
            if (!User.IsGuest())
                return true;

            var reservation = await _reservationService.GetByIdAsync(reservationId);
            return reservation.Success && reservation.Data!.GuestId == User.CurrentId();
        }

        private async Task<IActionResult> CreateView(NewReservationViewModel model)
        {
            if (User.IsGuest())
            {
                var me = await _guestService.GetByIdAsync(User.CurrentId());
                model.IsGuestAccount = true;
                model.GuestName = me.Success ? me.Data!.FullName : string.Empty;
            }
            else
            {
                var guests = await _guestService.GetAllAsync();
                model.KnownGuests = guests.Success ? guests.Data! : new();
            }

            return View("Create", model);
        }

        private ViewResult EditView(ReservationEditViewModel dto, int id)
        {
            dto.Id = id;
            return View("Edit", dto);
        }

        private static string RoomLabel(HotelBooking.Business.DTOs.Room.RoomDto r) =>
            $"Habitación {r.Number} · {r.RoomTypeName} · {r.RatePerNight:C}/noche";
    }
}
