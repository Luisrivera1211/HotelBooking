using HotelBooking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;
        private readonly ILogger<AuditController> _logger;

        public AuditController(IAuditService auditService, ILogger<AuditController> logger)
        {
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet("Recent")]
        public async Task<IActionResult> GetRecent([FromQuery] string? entityName, [FromQuery] int take = 200)
        {
            var result = await _auditService.GetRecentAsync(entityName, take);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching audit logs: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }

        [HttpGet("ByEntity/{entityName}/{entityId:int}")]
        public async Task<IActionResult> GetByEntity(string entityName, int entityId)
        {
            var result = await _auditService.GetByEntityAsync(entityName, entityId);

            if (result.Success)
                return Ok(result);

            _logger.LogError("Error fetching audit history: {ErrorMessage}", result.Message);
            return BadRequest(result);
        }
    }
}
