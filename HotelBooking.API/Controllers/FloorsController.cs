using HotelBooking.Business.DTOs.Floor;
using HotelBooking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FloorsController : ControllerBase
    {
        private readonly IFloorService _floorService;
        private readonly ILogger<FloorsController> _logger;

        public FloorsController(IFloorService floorService, ILogger<FloorsController> logger)
        {
            _floorService = floorService;
            _logger = logger;
        }

        [HttpGet("ByBlock/{blockId:int}")]
        public async Task<IActionResult> GetByBlock(int blockId)
        {
            var result = await _floorService.GetByBlockAsync(blockId);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching floors: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _floorService.GetByIdAsync(id);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching floor: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("CreateFloor")]
        public async Task<IActionResult> Post([FromBody] CreateFloorDto dto)
        {
            var result = await _floorService.CreateAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error creating floor: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("SetFloorActive/{id:int}")]
        public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive)
        {
            var result = await _floorService.SetActiveAsync(id, isActive);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error changing floor state: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
