using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data.Enums;

namespace HotelBooking.Data.Repositories
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        public RoomRepository(HotelBookingDbContext context) : base(context)
        { }

        public async Task<List<Room>> GetByFloorAsync(int floorId)
        {
            return await _context.Rooms.Where(f => f.FloorId == floorId).ToListAsync();
        }
        public async Task<bool> ExistsNumberInHotelAsync(int hotelId, string number)
        {
            return await _context.Rooms.Where(h => h.Floor.HotelId == hotelId && h.Number == number).AnyAsync();
        }
        public async Task<List<Room>> GetAvailableAsync(int hotelId, DateTime checkIn, DateTime checkOut, int guests)
        {
            return await _context.Rooms
                .Where(r => r.Floor.HotelId == hotelId
                && r.RoomType.Capacity >= guests
                && r.Floor.IsActive
                && r.Status == RoomStatus.Available
                && !r.Reservations.Any(t => t.CheckIn < checkOut && t.CheckOut > checkIn && t.Status != ReservationStatus.Cancelled))
                .Include(r => r.RoomType)
                .ToListAsync();

        }
    }
}
