using HotelBooking.Business.DTOs.Block;
using HotelBooking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlocksController : ControllerBase
    {
        private readonly IBlockService _blockService;
        private readonly ILogger<BlocksController> _logger;

        public BlocksController(IBlockService blockService, ILogger<BlocksController> logger)
        {
            _blockService = blockService;
            _logger = logger;
        }

        [HttpGet("ByHotel/{hotelId:int}")]
        public async Task<IActionResult> GetByHotel(int hotelId)
        {
            var result = await _blockService.GetByHotelAsync(hotelId);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching blocks: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _blockService.GetByIdAsync(id);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching block: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("CreateBlock")]
        public async Task<IActionResult> Post([FromBody] CreateBlockDto dto)
        {
            var result = await _blockService.CreateAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error creating block: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("ModifyBlock/{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateBlockDto dto)
        {
            var result = await _blockService.UpdateAsync(id, dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error modifying block: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("SetBlockActive/{id:int}")]
        public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive)
        {
            var result = await _blockService.SetActiveAsync(id, isActive);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error changing block state: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
