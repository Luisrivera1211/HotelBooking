using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;


namespace HotelBooking.Data.Repositories
{
    public class GuestRepository : BaseRepository<Guest>, IGuestRepository 
    {
        public GuestRepository(HotelBookingDbContext context) : base(context)
        { }
    }
}
