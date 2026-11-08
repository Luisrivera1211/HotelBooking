using HotelBooking.Data.Entities;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Interfaces
{
    public interface IGuestRepository : IBaseRepository<Guest>
    {
        Task<OperationResult<Guest?>> GetByDocumentAsync(string document);
        Task<OperationResult<bool>> ExistsDocumentAsync(string document, int? excludeGuestId = null);
    }
}