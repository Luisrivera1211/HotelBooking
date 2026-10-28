using HotelBooking.Data.Base;


namespace HotelBooking.Data.Interfaces
{
    public interface IBaseRepository<T> where T : AuditEntity
    {
        Task<T?> GetEntityByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> SaveEntityAsync(T entity);
        Task<T> UpdateEntityAsync(T entity);
        Task<bool> ExistsAsync(int id);
    }
}
