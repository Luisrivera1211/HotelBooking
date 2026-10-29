using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace HotelBooking.Data.Repositories
{
    public class GuestRepository : BaseRepository<Guest>, IGuestRepository
    {
        public GuestRepository(HotelBookingDbContext context) : base(context)
        { }

        public async Task<Guest?> GetByDocumentAsync(string document)
        {
            return await _context.Guests.FirstOrDefaultAsync(g => g.Document == document);
        }
    }
}
