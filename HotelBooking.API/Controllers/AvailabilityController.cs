using HotelBooking.Business.DTOs.Availability;
using HotelBooking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly ILogger<AvailabilityController> _logger;

        public AvailabilityController(IAvailabilityService availabilityService, ILogger<AvailabilityController> logger)
        {
            _availabilityService = availabilityService;
            _logger = logger;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search([FromBody] AvailabilitySearchDto dto)
        {
            var result = await _availabilityService.SearchAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error searching availability: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
