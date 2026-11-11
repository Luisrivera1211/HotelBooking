using HotelBooking.Business.DTOs.Hotels;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [Authorize(Roles = Roles.Administrator)]
    public class HotelsController : AppController
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _hotelService.GetAllAsync();
            if (!result.Success)
            {
                ShowError(result.Message);
                return View(new List<HotelDto>());
            }

            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return FormView(new HotelFormViewModel(), "Nuevo hotel", nameof(Create), null);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HotelFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return FormView(dto, "Nuevo hotel", nameof(Create), null);

            var result = await _hotelService.CreateAsync(dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return FormView(dto, "Nuevo hotel", nameof(Create), null);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _hotelService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectError(result.Message, nameof(Index));

            var dto = new HotelFormViewModel { Name = result.Data!.Name, Address = result.Data.Address };
            return FormView(dto, "Editar hotel", nameof(Edit), id);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, HotelFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return FormView(dto, "Editar hotel", nameof(Edit), id);

            var result = await _hotelService.UpdateAsync(id, dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return FormView(dto, "Editar hotel", nameof(Edit), id);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id, bool isActive)
        {
            var result = await _hotelService.SetActiveAsync(id, isActive);
            return FlashAndRedirect(result, isActive ? "Hotel activado." : "Hotel desactivado.", nameof(Index));
        }

        private ViewResult FormView(HotelFormViewModel dto, string title, string action, int? id)
        {
            ViewData["Title"] = title;
            ViewData["FormAction"] = action;
            ViewData["Id"] = id;
            return View("Form", dto);
        }
    }
}
