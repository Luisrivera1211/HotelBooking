using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(HotelBookingDbContext context) : base(context)
        { }

        protected override string? Validate(Notification notification)
        {
            if (string.IsNullOrWhiteSpace(notification.Message))
                return DataMessages.Required("Mensaje");
            if (notification.Message.Length > 255)
                return DataMessages.TooLong("Mensaje", 255);
            return null;
        }

        public async Task<OperationResult<List<Notification>>> GetByReservationAsync(int reservationId)
        {
            try
            {
                if (reservationId <= 0)
                    return OperationResult<List<Notification>>.Validation(DataMessages.InvalidId);

                var notifications = await _context.Notifications
                    .AsNoTracking()
                    .Where(n => n.ReservationId == reservationId)
                    .ToListAsync();
                return OperationResult<List<Notification>>.Ok(notifications);
            }
            catch (Exception)
            {
                return OperationResult<List<Notification>>.Unexpected(DataMessages.QueryError);
            }
        }

        public async Task<OperationResult<bool>> ExistsSentAsync(int reservationId, NotificationType type)
        {
            try
            {
                if (reservationId <= 0)
                    return OperationResult<bool>.Validation(DataMessages.InvalidId);

                var exists = await _context.Notifications.AnyAsync(n =>
                    n.ReservationId == reservationId
                    && n.Type == type
                    && n.Status == NotificationStatus.Sent);
                return OperationResult<bool>.Ok(exists);
            }
            catch (Exception)
            {
                return OperationResult<bool>.Unexpected(DataMessages.QueryError);
            }
        }
    }
}
