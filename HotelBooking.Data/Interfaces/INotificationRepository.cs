using HotelBooking.Data.Entities;

namespace HotelBooking.Data.Interfaces
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<List<Notification>> GetByReservationId(int reservationId);

        Task<bool> ExistsSentAsync (int reservationId, string type);
    }
}
