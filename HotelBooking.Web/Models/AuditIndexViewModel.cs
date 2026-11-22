using HotelBooking.Business.DTOs.Audit;

namespace HotelBooking.Web.Models
{
    public class AuditIndexViewModel
    {
        public string? Entity { get; set; }
        public List<AuditLogDto> Logs { get; set; } = new();
    }
}
