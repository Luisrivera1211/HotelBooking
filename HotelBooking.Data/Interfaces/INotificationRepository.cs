using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Interfaces
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<OperationResult<List<Notification>>> GetByReservationAsync(int reservationId);
        Task<OperationResult<bool>> ExistsSentAsync(int reservationId, NotificationType type);
    }
}