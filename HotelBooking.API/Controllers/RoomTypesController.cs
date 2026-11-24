using HotelBooking.Business.DTOs.RoomType;
using HotelBooking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypesController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly ILogger<RoomTypesController> _logger;

        public RoomTypesController(IRoomTypeService roomTypeService, ILogger<RoomTypesController> logger)
        {
            _roomTypeService = roomTypeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _roomTypeService.GetAllAsync();

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching room types: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _roomTypeService.GetByIdAsync(id);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching room type: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("CreateRoomType")]
        public async Task<IActionResult> Post([FromBody] CreateRoomTypeDto dto)
        {
            var result = await _roomTypeService.CreateAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error creating room type: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("ModifyRoomType/{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateRoomTypeDto dto)
        {
            var result = await _roomTypeService.UpdateAsync(id, dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error modifying room type: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
