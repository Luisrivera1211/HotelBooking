
using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(HotelBookingDbContext context) : base(context)
        { }

        public async Task<List<Reservation>> GetOverlappingAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeReservationId = null)
        {

            var query = _context.Reservations
                .Where(r => r.RoomId == roomId && r.CheckIn < checkOut && r.CheckOut > checkIn && r.Status != ReservationStatus.Cancelled);

            if (excludeReservationId.HasValue)
                query = query.Where(r => r.Id != excludeReservationId);
            

            return await query.ToListAsync();

        }

        public async Task<List<Reservation>> GetByGuestAsync(int guestId)
        {
            return await _context.Reservations.Where(n => n.GuestId == guestId).ToListAsync();
        }

        public async Task<Reservation?> GetWithDetailsAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();
        }

    }
}
