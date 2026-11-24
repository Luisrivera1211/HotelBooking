using HotelBooking.Business.Interfaces;
using HotelBooking.Data.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet("ByReservation/{reservationId:int}")]
        public async Task<IActionResult> GetByReservation(int reservationId)
        {
            var result = await _notificationService.GetByReservationAsync(reservationId);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching notifications: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpPost("Notify/{reservationId:int}")]
        public async Task<IActionResult> Notify(int reservationId, [FromQuery] NotificationType type)
        {
            var result = await _notificationService.NotifyAsync(reservationId, type);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error sending notification: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
