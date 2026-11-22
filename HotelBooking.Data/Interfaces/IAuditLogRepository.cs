using HotelBooking.Data.Entities;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<OperationResult<List<AuditLog>>> GetRecentAsync(string? entityName, int take);
        Task<OperationResult<List<AuditLog>>> GetByEntityAsync(string entityName, int entityId);
    }
}
