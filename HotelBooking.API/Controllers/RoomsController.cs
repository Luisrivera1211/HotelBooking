using HotelBooking.Business.DTOs.Room;
using HotelBooking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        [HttpGet("ByFloor/{floorId:int}")]
        public async Task<IActionResult> GetByFloor(int floorId)
        {
            var result = await _roomService.GetByFloorAsync(floorId);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching rooms: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _roomService.GetByIdAsync(id);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching room: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("CreateRoom")]
        public async Task<IActionResult> Post([FromBody] CreateRoomDto dto)
        {
            var result = await _roomService.CreateAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error creating room: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("ModifyRoom/{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateRoomDto dto)
        {
            var result = await _roomService.UpdateAsync(id, dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error modifying room: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("ChangeRoomStatus/{id:int}")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeRoomStatusDto dto)
        {
            var result = await _roomService.ChangeStatusAsync(id, dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error changing room status: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
