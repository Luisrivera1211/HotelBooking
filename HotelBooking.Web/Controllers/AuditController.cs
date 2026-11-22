using HotelBooking.Business.DTOs.Audit;
using HotelBooking.Business.Interfaces;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [Authorize(Roles = Roles.Administrator)]
    public class AuditController : AppController
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task<IActionResult> Index(string? entity)
        {
            var result = await _auditService.GetRecentAsync(entity);
            if (!result.Success)
                ShowError(result.Message);

            return View(new AuditIndexViewModel
            {
                Entity = entity,
                Logs = result.Success ? result.Data! : new List<AuditLogDto>()
            });
        }
    }
}
