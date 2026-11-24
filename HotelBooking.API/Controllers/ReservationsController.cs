using HotelBooking.Business.DTOs.Reservation;
using HotelBooking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(IReservationService reservationService, ILogger<ReservationsController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _reservationService.GetByIdAsync(id);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching reservation: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpGet("ByGuest/{guestId:int}")]
        public async Task<IActionResult> GetByGuest(int guestId)
        {
            var result = await _reservationService.GetByGuestAsync(guestId);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching reservations by guest: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("CreateReservation")]
        public async Task<IActionResult> Post([FromBody] CreateReservationDto dto)
        {
            var result = await _reservationService.CreateAsync(dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error creating reservation: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("ModifyReservation/{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ModifyReservationDto dto)
        {
            var result = await _reservationService.ModifyAsync(id, dto);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error modifying reservation: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("ConfirmReservation/{id:int}")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _reservationService.ConfirmAsync(id);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error confirming reservation: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("CancelReservation/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _reservationService.CancelAsync(id);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error cancelling reservation: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
