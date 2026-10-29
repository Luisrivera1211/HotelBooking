
using HotelBooking.Data.Entities;

namespace HotelBooking.Data.Interfaces
{
    public interface IGuestRepository : IBaseRepository<Guest>
    {
        Task<Guest?> GetByDocumentAsync(string document);

    }
}
