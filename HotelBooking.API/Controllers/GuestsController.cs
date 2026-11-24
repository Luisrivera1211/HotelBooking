using HotelBooking.Business.DTOs.Employee;
using HotelBooking.Business.DTOs.Guest;
using HotelBooking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestsController : ControllerBase
    {
        private readonly IGuestService _guestService;
        private readonly ILogger<GuestsController> _logger;

        public GuestsController(IGuestService guestService, ILogger<GuestsController> logger)
        {
            _guestService = guestService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _guestService.GetAllAsync();

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching guests: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _guestService.GetByIdAsync(id);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching guest: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpGet("ByDocument/{document}")]
        public async Task<IActionResult> GetByDocument(string document)
        {
            var result = await _guestService.GetByDocumentAsync(document);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching guest by document: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("CreateGuest")]
        public async Task<IActionResult> Post([FromBody] CreateGuestDto dto)
        {
            var result = await _guestService.CreateAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error creating guest: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("ModifyGuest/{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateGuestDto dto)
        {
            var result = await _guestService.UpdateAsync(id, dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error modifying guest: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("RegisterGuestAccount")]
        public async Task<IActionResult> RegisterAccount([FromBody] RegisterGuestAccountDto dto)
        {
            var result = await _guestService.RegisterAccountAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error registering guest account: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("LoginGuest")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _guestService.LoginAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error guest login: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
