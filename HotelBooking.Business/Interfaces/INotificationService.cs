using HotelBooking.Data.Entities;

namespace HotelBooking.Business.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> GenerateNotificationAsync();
        Task<Notification> SendNotificationAsync();
        Task<Notification> MarkAsSent();
        Task<Notification> MarkAsFailed();
        Task<Notification> AvoidDuplicatedAsync();

    }
}
