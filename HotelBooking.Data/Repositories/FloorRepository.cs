using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class FloorRepository : BaseRepository<Floor>, IFloorRepository
    {
        public FloorRepository(HotelBookingDbContext context) : base(context)
        { }



    }
}
