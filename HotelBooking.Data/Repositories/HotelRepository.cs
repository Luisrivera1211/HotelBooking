using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class HotelRepository : BaseRepository<Hotel>, IHotelRepository
    {
        public HotelRepository(HotelBookingDbContext context) : base(context)
        { }

        protected override string? Validate(Hotel hotel)
        {
            if (string.IsNullOrWhiteSpace(hotel.Name))
                return DataMessages.Required("Nombre");
            if (hotel.Name.Length > 50)
                return DataMessages.TooLong("Nombre", 50);
            if (string.IsNullOrWhiteSpace(hotel.Address))
                return DataMessages.Required("Dirección");
            if (hotel.Address.Length > 100)
                return DataMessages.TooLong("Dirección", 100);
            return null;
        }

        public async Task<OperationResult<List<Hotel>>> GetActiveHotelsAsync()
        {
            try
            {
                var hotels = await _context.Hotels
                    .AsNoTracking()
                    .Where(h => h.IsActive)
                    .ToListAsync();
                return OperationResult<List<Hotel>>.Ok(hotels);
            }
            catch (Exception)
            {
                return OperationResult<List<Hotel>>.Unexpected(DataMessages.QueryError);
            }
        }

        public async Task<OperationResult<Hotel?>> GetByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return OperationResult<Hotel?>.Validation(DataMessages.Required("Nombre"));

                var hotel = await _context.Hotels
                    .AsNoTracking()
                    .FirstOrDefaultAsync(h => h.Name == name.Trim());
                return OperationResult<Hotel?>.Ok(hotel);
            }
            catch (Exception)
            {
                return OperationResult<Hotel?>.Unexpected(DataMessages.QueryError);
            }
        }
    }
}
