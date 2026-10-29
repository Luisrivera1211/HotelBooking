using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Context;


namespace HotelBooking.Data.Repositories
{

    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(HotelBookingDbContext context) : base(context)
        { }

       public async Task<List<Notification>> GetByReservationIdAsync(int reservationId)
        {
            return await _context.Notifications.Where(n => n.ReservationId == reservationId).ToListAsync();
        }

       public async Task<bool> ExistsSentAsync(int reservationId, string type)

        {
            return await _context.Notifications.Where(n => n.ReservationId == reservationId && n.Type == type && n.Status == NotificationStatus.Sent).AnyAsync();
        }
    }
}