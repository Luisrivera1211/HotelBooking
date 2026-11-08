using HotelBooking.Data.Entities;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Interfaces
{
    public interface IHotelRepository : IBaseRepository<Hotel>
    {
        Task <OperationResult<List<Hotel>>> GetActiveHotelsAsync();

        Task <OperationResult<Hotel?>> GetByNameAsync(string name);

    }
}
