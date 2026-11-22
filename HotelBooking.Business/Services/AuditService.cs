using HotelBooking.Business.DTOs.Audit;
using HotelBooking.Business.Interfaces;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Services
{
    public class AuditService : IAuditService
    {
        private static readonly string[] AllowedEntities =
        {
            nameof(Reservation), nameof(Room), nameof(Notification)
        };

        private readonly IAuditLogRepository _auditLogRepository;

        public AuditService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<OperationResult<List<AuditLogDto>>> GetRecentAsync(string? entityName, int take = 200)
        {
            var filter = entityName?.Trim();

            if (!string.IsNullOrEmpty(filter) && !AllowedEntities.Contains(filter))
                filter = null;

            var logs = await _auditLogRepository.GetRecentAsync(filter, take);
            if (!logs.Success)
                return OperationResult<List<AuditLogDto>>.Fail(logs.Message, logs.ErrorType);

            return OperationResult<List<AuditLogDto>>.Ok(logs.Data!.Select(ToDto).ToList());
        }

        public async Task<OperationResult<List<AuditLogDto>>> GetByEntityAsync(string entityName, int entityId)
        {
            var name = entityName?.Trim() ?? string.Empty;
            if (!AllowedEntities.Contains(name))
                return OperationResult<List<AuditLogDto>>.Validation("Entidad no auditada.");

            var logs = await _auditLogRepository.GetByEntityAsync(name, entityId);
            if (!logs.Success)
                return OperationResult<List<AuditLogDto>>.Fail(logs.Message, logs.ErrorType);

            return OperationResult<List<AuditLogDto>>.Ok(logs.Data!.Select(ToDto).ToList());
        }

        private static AuditLogDto ToDto(AuditLog a) => new()
        {
            Id = a.Id,
            EntityName = a.EntityName,
            EntityId = a.EntityId,
            Action = a.Action,
            UserName = a.UserName,
            OccurredAt = a.OccurredAt,
            Details = a.Details
        };
    }
}
