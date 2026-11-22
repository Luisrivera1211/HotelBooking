using HotelBooking.Business.DTOs.Block;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [Authorize(Roles = Roles.Administrator)]
    public class BlocksController : AppController
    {
        private readonly IBlockService _blockService;
        private readonly IHotelService _hotelService;

        public BlocksController(IBlockService blockService, IHotelService hotelService)
        {
            _blockService = blockService;
            _hotelService = hotelService;
        }
        public async Task<IActionResult> Index(int hotelId)
        {
            var hotel = await _hotelService.GetByIdAsync(hotelId);
            if (!hotel.Success)
                return RedirectError(hotel.Message, "Index", "Hotels");

            var blocks = await _blockService.GetByHotelAsync(hotelId);
            if (!blocks.Success)
                ShowError(blocks.Message);

            return View(new BlocksIndexViewModel
            {
                Hotel = hotel.Data!,
                Blocks = blocks.Success ? blocks.Data! : new List<BlockDto>()
            });
        }

        [HttpGet]
        public IActionResult Create(int hotelId)
        {
            return FormView(new BlockFormViewModel { HotelId = hotelId }, null);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BlockFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return FormView(dto, null);

            var result = await _blockService.CreateAsync(dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return FormView(dto, null);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Index), new { hotelId = dto.HotelId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _blockService.GetByIdAsync(id);
            if (!result.Success)
                return RedirectError(result.Message, "Index", "Hotels");

            var dto = new BlockFormViewModel { Name = result.Data!.Name, HotelId = result.Data.HotelId };
            return FormView(dto, id);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BlockFormViewModel dto)
        {
            if (!ModelState.IsValid)
                return FormView(dto, id);

            var result = await _blockService.UpdateAsync(id, dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return FormView(dto, id);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Index), new { hotelId = dto.HotelId });
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id, bool isActive, int hotelId)
        {
            var result = await _blockService.SetActiveAsync(id, isActive);
            return FlashAndRedirect(result, isActive ? "Bloque activado." : "Bloque desactivado.",
                nameof(Index), routeValues: new { hotelId });
        }

        private ViewResult FormView(BlockFormViewModel dto, int? id)
        {
            dto.Id = id;
            return View("Form", dto);
        }
    }
}
