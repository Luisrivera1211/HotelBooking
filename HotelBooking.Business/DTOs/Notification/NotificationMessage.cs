using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;

namespace HotelBooking.Business.DTOs.Notification
{
    public record NotificationMessage(NotificationType Type, string Subject, string Summary, ReservationStatus Reservation);
}