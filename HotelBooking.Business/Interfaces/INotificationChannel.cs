using HotelBooking.Business.DTOs.Notification;
using HotelBooking.Data.Entities;

namespace HotelBooking.Business.Interfaces
{
    public interface INotificationChannel
    {
        string Name { get; }                                 
        bool CanDeliverTo(Guest guest);                      
        Task<bool> SendAsync(Guest guest, NotificationMessage message);  
    }
}