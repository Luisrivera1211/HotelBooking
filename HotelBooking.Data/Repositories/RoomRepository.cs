using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;
using Microsoft.EntityFrameworkCore;


namespace HotelBooking.Data.Repositories
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        public RoomRepository(HotelBookingDbContext context) : base(context)
        { }

        protected override string? Validate(Room room)
        {
            if (string.IsNullOrWhiteSpace(room.Number))
                return DataMessages.Required("Número");
            if (room.Number.Length > 50)
                return DataMessages.TooLong("Número", 50);
            return null;
        }

        public async Task<OperationResult<List<Room>>> GetByFloorAsync(int floorId)
        {
            try
            {
                if (floorId <= 0)
                    return OperationResult<List<Room>>.Validation(DataMessages.InvalidId);

                var rooms = await _context.Rooms
                    .AsNoTracking()
                    .Where(r => r.FloorId == floorId)
                    .ToListAsync();
                return OperationResult<List<Room>>.Ok(rooms);
            }
            catch (Exception)
            {
                return OperationResult<List<Room>>.Unexpected(DataMessages.QueryError);
            }
        }

        public async Task<OperationResult<bool>> ExistsNumberInHotelAsync(int hotelId, string number, int? excludeRoomId = null)
        {
            try
            {
                if (hotelId <= 0)
                    return OperationResult<bool>.Validation(DataMessages.InvalidId);
                if (string.IsNullOrWhiteSpace(number))
                    return OperationResult<bool>.Validation(DataMessages.Required("Numero"));

                var query = _context.Rooms.Where(r => r.Floor.Block.HotelId == hotelId && r.Number == number.Trim());

                if (excludeRoomId.HasValue)
                    query = query.Where(r => r.Id != excludeRoomId.Value);

                return OperationResult<bool>.Ok(await query.AnyAsync());
            }
            catch (Exception)
            {
                return OperationResult<bool>.Unexpected(DataMessages.QueryError);
            }
        }
        

        public async Task<OperationResult<List<Room>>> GetAvailableAsync(int hotelId, DateTime checkIn, DateTime checkOut, int guests)
        {
            try
            {
                if (hotelId <= 0)
                    return OperationResult<List<Room>>.Validation(DataMessages.InvalidId);
                if (checkOut <= checkIn)
                    return OperationResult<List<Room>>.Validation("La fecha de salida debe ser posterior a la de entrada.");  
                if (guests <= 0)
                    return OperationResult<List<Room>>.Validation(DataMessages.MustBePositive("Cantidad de huéspedes"));

                var rooms = await _context.Rooms
                    .AsNoTracking()
                    .Include(r => r.RoomType)
                    .Where(r => r.Floor.Block.HotelId == hotelId
                    && r.Floor.IsActive
                    && r.RoomType.Capacity >= guests
                    && r.Status == RoomStatus.Available
                    && !r.Reservations.Any(x => x.CheckIn < checkOut
                                           && x.CheckOut > checkIn
                                           && x.Status != ReservationStatus.Cancelled))
                    .ToListAsync();

                return OperationResult<List<Room>>.Ok(rooms);
            }
            catch (Exception)
            {
                return OperationResult<List<Room>>.Unexpected(DataMessages.QueryError);
            }
        }
    }
}
