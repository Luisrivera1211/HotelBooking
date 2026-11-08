using HotelBooking.Data.Base;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Interfaces
{
    public interface IBaseRepository<T> where T : AuditEntity
    {
        Task<OperationResult<T?>> GetEntityByIdAsync(int id);
        Task<OperationResult<List<T>>> GetAllAsync();
        Task<OperationResult<T>> SaveEntityAsync(T entity);
        Task<OperationResult<T>> UpdateEntityAsync(T entity);
        Task<OperationResult<bool>> ExistsAsync(int id);
    }
}
