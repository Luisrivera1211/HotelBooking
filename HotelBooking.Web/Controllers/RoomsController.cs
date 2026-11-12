using HotelBooking.Web.Helpers;
using HotelBooking.Business.DTOs.Room;
using HotelBooking.Business.Interfaces;
using HotelBooking.Data.Enums;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBooking.Web.Controllers
{
    [Authorize(Roles = Roles.Administrator)]
    public class RoomsController : AppController
    {
        private readonly IRoomService _roomService;
        private readonly IFloorService _floorService;
        private readonly IRoomTypeService _roomTypeService;

        public RoomsController(IRoomService roomService, IFloorService floorService, IRoomTypeService roomTypeService)
        {
            _roomService = roomService;
            _floorService = floorService;
            _roomTypeService = roomTypeService;
        }

        public async Task<IActionResult> Index(int floorId)
        {
            var floor = await _floorService.GetByIdAsync(floorId);
            if (!floor.Success)
                return RedirectError(floor.Message, "Index", "Hotels");

            var rooms = await _roomService.GetByFloorAsync(floorId);
            if (!rooms.Success)
                ShowError(rooms.Message);

            ViewData["Floor"] = floor.Data;
            return View(rooms.Success ? rooms.Data : new List<RoomDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Create(int floorId)
        {
            return await FormView(new RoomFormViewModel { FloorId = floorId }, "Nueva habitación", nameof(Create), null);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoomFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return await FormView(dto, "Nueva habitación", nameof(Create), null);

            var result = await _roomService.CreateAsync(dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return await FormView(dto, "Nueva habitación", nameof(Create), null);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Index), new { floorId = dto.FloorId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _roomService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectError(result.Message, "Index", "Hotels");

            var dto = new RoomFormViewModel
            {
                FloorId = result.Data!.FloorId,
                RoomTypeId = result.Data.RoomTypeId,
                Number = result.Data.Number
            };
            return await FormView(dto, "Editar habitación", nameof(Edit), id);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, RoomFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return await FormView(dto, "Editar habitación", nameof(Edit), id);

            var result = await _roomService.UpdateAsync(id, dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return await FormView(dto, "Editar habitación", nameof(Edit), id);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Index), new { floorId = dto.FloorId });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, RoomStatus status, int floorId)
        {
            var result = await _roomService.ChangeStatusAsync(id, new ChangeRoomStatusDto { Status = status });
            return FlashAndRedirect(result, "Estado actualizado.", nameof(Index), routeValues: new { floorId });
        }

        private async Task<ViewResult> FormView(RoomFormViewModel dto, string title, string action, int? id)
        {
            var types = await _roomTypeService.GetAllAsync();
            if (!types.Success)
                ShowError(types.Message);

            ViewData["RoomTypes"] = (types.Success ? types.Data! : new()).Select(t =>
                new SelectListItem($"{t.Name} (cap. {t.Capacity}, {t.RatePerNight.ToDop()}/noche)", t.Id.ToString())).ToList();
            ViewData["Title"] = title;
            ViewData["FormAction"] = action;
            ViewData["Id"] = id;
            return View("Form", dto);
        }
    }
}
