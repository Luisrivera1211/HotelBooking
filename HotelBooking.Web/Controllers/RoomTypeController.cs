using HotelBooking.Business.DTOs.RoomType;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [Authorize(Roles = Roles.Administrator)]
    public class RoomTypesController : AppController
    {
        private readonly IRoomTypeService _roomTypeService;

        public RoomTypesController(IRoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _roomTypeService.GetAllAsync();
            if (!result.Success)
            {
                ShowError(result.Message);
                return View(new List<RoomTypeDto>());
            }

            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return FormView(new RoomTypeFormViewModel(), "Nuevo tipo de habitación", nameof(Create), null);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoomTypeFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return FormView(dto, "Nuevo tipo de habitación", nameof(Create), null);

            var result = await _roomTypeService.CreateAsync(dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return FormView(dto, "Nuevo tipo de habitación", nameof(Create), null);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _roomTypeService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectError(result.Message, nameof(Index));

            var dto = new RoomTypeFormViewModel
            {
                Name = result.Data!.Name,
                Capacity = result.Data.Capacity,
                RatePerNight = result.Data.RatePerNight
            };
            return FormView(dto, "Editar tipo de habitación", nameof(Edit), id);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, RoomTypeFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return FormView(dto, "Editar tipo de habitación", nameof(Edit), id);

            var result = await _roomTypeService.UpdateAsync(id, dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return FormView(dto, "Editar tipo de habitación", nameof(Edit), id);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private ViewResult FormView(RoomTypeFormViewModel dto, string title, string action, int? id)
        {
            ViewData["Title"] = title;
            ViewData["FormAction"] = action;
            ViewData["Id"] = id;
            return View("Form", dto);
        }
    }
}
