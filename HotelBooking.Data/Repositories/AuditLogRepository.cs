using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly HotelBookingDbContext _context;

        public AuditLogRepository(HotelBookingDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<List<AuditLog>>> GetRecentAsync(string? entityName, int take)
        {
            try
            {
                take = Math.Clamp(take, 1, 500);

                var query = _context.AuditLogs.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(entityName))
                    query = query.Where(a => a.EntityName == entityName);

                var logs = await query
                    .OrderByDescending(a => a.OccurredAt)
                    .ThenByDescending(a => a.Id)
                    .Take(take)
                    .ToListAsync();

                return OperationResult<List<AuditLog>>.Ok(logs);
            }
            catch (Exception)
            {
                return OperationResult<List<AuditLog>>.Unexpected(DataMessages.QueryError);
            }
        }

        public async Task<OperationResult<List<AuditLog>>> GetByEntityAsync(string entityName, int entityId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entityName))
                    return OperationResult<List<AuditLog>>.Validation(DataMessages.Required("Entidad"));
                if (entityId <= 0)
                    return OperationResult<List<AuditLog>>.Validation(DataMessages.InvalidId);

                var logs = await _context.AuditLogs
                    .AsNoTracking()
                    .Where(a => a.EntityName == entityName && a.EntityId == entityId)
                    .OrderByDescending(a => a.OccurredAt)
                    .ThenByDescending(a => a.Id)
                    .ToListAsync();

                return OperationResult<List<AuditLog>>.Ok(logs);
            }
            catch (Exception)
            {
                return OperationResult<List<AuditLog>>.Unexpected(DataMessages.QueryError);
            }
        }
    }
}
