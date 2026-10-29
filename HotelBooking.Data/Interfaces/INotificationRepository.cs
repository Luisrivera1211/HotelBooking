using HotelBooking.Data.Entities;

namespace HotelBooking.Data.Interfaces
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<List<Notification>> GetByReservationIdAsync(int reservationId);

        Task<bool> ExistsSentAsync(int reservationId, string type);
    }
}
