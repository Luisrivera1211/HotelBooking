using HotelBooking.Data.Entities;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<OperationResult<Employee?>> GetByEmailAsync(string email);
        Task<OperationResult<bool>> ExistsEmailAsync(string email, int? excludeEmployeeId = null);
    }
}
