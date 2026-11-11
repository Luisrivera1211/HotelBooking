using HotelBooking.Business.DTOs.Floor;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [Authorize(Roles = Roles.Administrator)]
    public class FloorsController : AppController
    {
        private readonly IFloorService _floorService;
        private readonly IBlockService _blockService;

        public FloorsController(IFloorService floorService, IBlockService blockService)
        {
            _floorService = floorService;
            _blockService = blockService;
        }

        // Pisos de un bloque
        public async Task<IActionResult> Index(int blockId)
        {
            var block = await _blockService.GetByIdAsync(blockId);
            if (!block.Success)
                return RedirectError(block.Message, "Index", "Hotels");

            var floors = await _floorService.GetByBlockAsync(blockId);
            if (!floors.Success)
                ShowError(floors.Message);

            ViewData["Block"] = block.Data;
            return View(floors.Success ? floors.Data : new List<FloorDto>());
        }

        [HttpGet]
        public IActionResult Create(int blockId)
        {
            ViewData["Title"] = "Nuevo piso";
            return View(new FloorFormViewModel { BlockId = blockId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(FloorFormViewModel dto)
        {
            ViewData["Title"] = "Nuevo piso";
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _floorService.CreateAsync(dto.ToDto());
            if (!result.Success)
            {
                AddError(result);
                return View(dto);
            }

            FlashOk(result.Message);
            return RedirectToAction(nameof(Index), new { blockId = dto.BlockId });
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id, bool isActive, int blockId)
        {
            var result = await _floorService.SetActiveAsync(id, isActive);
            return FlashAndRedirect(result, isActive ? "Piso activado." : "Piso desactivado.",
                nameof(Index), routeValues: new { blockId });
        }
    }
}
