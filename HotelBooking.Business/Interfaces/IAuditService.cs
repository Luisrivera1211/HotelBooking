using HotelBooking.Business.DTOs.Audit;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface IAuditService
    {
        Task<OperationResult<List<AuditLogDto>>> GetRecentAsync(string? entityName, int take = 200);

        Task<OperationResult<List<AuditLogDto>>> GetByEntityAsync(string entityName, int entityId);
    }
}
