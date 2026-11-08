using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class GuestRepository : BaseRepository<Guest>, IGuestRepository
    {
        public GuestRepository(HotelBookingDbContext context) : base(context)
        { }

        protected override string? Validate(Guest guest)
        {
            if (string.IsNullOrWhiteSpace(guest.FullName))
                return DataMessages.Required("Nombre");
            if (guest.FullName.Length > 100)
                return DataMessages.TooLong("Nombre", 100);
            if (string.IsNullOrWhiteSpace(guest.Document))
                return DataMessages.Required("Documento");
            if (guest.Document.Length > 50)
                return DataMessages.TooLong("Documento", 50);
            if (string.IsNullOrWhiteSpace(guest.Email))
                return DataMessages.Required("Correo");
            if (guest.Email.Length > 100)
                return DataMessages.TooLong("Correo", 100);
            if (string.IsNullOrWhiteSpace(guest.Phone))
                return DataMessages.Required("Teléfono");
            if (guest.Phone.Length > 50)
                return DataMessages.TooLong("Teléfono", 50);
            return null;
        }

        public async Task<OperationResult<Guest?>> GetByDocumentAsync(string document)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(document))
                    return OperationResult<Guest?>.Validation(DataMessages.Required("Documento"));

                var guest = await _context.Guests
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Document == document.Trim());
                return OperationResult<Guest?>.Ok(guest);
            }
            catch (Exception)
            {
                return OperationResult<Guest?>.Unexpected(DataMessages.QueryError);
            }
        }

        public async Task<OperationResult<bool>> ExistsDocumentAsync(string document, int? excludeGuestId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(document))
                    return OperationResult<bool>.Validation(DataMessages.Required("Documento"));

                var query = _context.Guests.Where(g => g.Document == document.Trim());

                if (excludeGuestId.HasValue)
                    query = query.Where(g => g.Id != excludeGuestId.Value);

                return OperationResult<bool>.Ok(await query.AnyAsync());
            }
            catch (Exception)
            {
                return OperationResult<bool>.Unexpected(DataMessages.QueryError);
            }
        }
    }
}