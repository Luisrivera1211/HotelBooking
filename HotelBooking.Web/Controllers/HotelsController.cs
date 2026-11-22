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
            return FormView(new HotelFormViewModel(), null);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HotelFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return FormView(dto, null);

            var result = await _hotelService.CreateAsync(dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return FormView(dto, null);
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

            var dto = new HotelFormViewModel { Name = result.Data!.Name, Address = result.Data.Address, Stars = result.Data.Stars };
            return FormView(dto, id);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, HotelFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return FormView(dto, id);

            var result = await _hotelService.UpdateAsync(id, dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return FormView(dto, id);
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

        private ViewResult FormView(HotelFormViewModel dto, int? id)
        {
            dto.Id = id;
            return View("Form", dto);
        }
    }
}
