using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Context;

namespace HotelBooking.Data.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(HotelBookingDbContext context) : base(context)
        { }
    }
}
