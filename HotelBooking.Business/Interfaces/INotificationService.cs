using HotelBooking.Business.DTOs.Notification;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface INotificationService
    {
        Task<OperationResult<List<NotificationDto>>> NotifyAsync(int reservationId, NotificationType type);
        Task<OperationResult<List<NotificationDto>>> GetByReservationAsync(int reservationId);
    }
}


