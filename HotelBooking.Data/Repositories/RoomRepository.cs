using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;

namespace HotelBooking.Data.Repositories
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        public RoomRepository(HotelBookingDbContext context) : base(context)
        { }
    }
}
