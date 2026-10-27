using HotelBooking.Data.Entities;

namespace HotelBooking.Data.Interfaces
{
    public interface IHotelRepository : IBaseRepository<Hotel>
    {
        Task<List<Hotel>> GetActiveHotelsAsync();

        Task<Hotel?> GetByNameAsync(string name);

    }
}
