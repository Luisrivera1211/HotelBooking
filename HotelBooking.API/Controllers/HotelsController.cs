using HotelBooking.Business.DTOs.Hotels;
using HotelBooking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(IHotelService hotelService, ILogger<HotelsController> logger)
        {
            _hotelService = hotelService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _hotelService.GetAllAsync();

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching hotels: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _hotelService.GetByIdAsync(id);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching hotel: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("CreateHotel")]
        public async Task<IActionResult> Post([FromBody] CreateHotelDto dto)
        {
            var result = await _hotelService.CreateAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error creating hotel: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("ModifyHotel/{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateHotelDto dto)
        {
            var result = await _hotelService.UpdateAsync(id, dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error modifying hotel: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("SetHotelActive/{id:int}")]
        public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive)
        {
            var result = await _hotelService.SetActiveAsync(id, isActive);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error changing hotel state: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
