using HotelBooking.Business.DTOs.Employee;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface IEmployeeService
    {
        Task<OperationResult<EmployeeDto>> RegisterAsync(RegisterEmployeeDto dto);
        Task<OperationResult<EmployeeDto>> LoginAsync(LoginDto dto);
    }
}
