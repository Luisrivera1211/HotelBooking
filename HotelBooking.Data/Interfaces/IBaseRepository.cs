using HotelBooking.Data.Base;


namespace HotelBooking.Data.Interfaces
{
    public interface IBaseRepository<T> where T : AuditEntity
    {
        Task<T?> GetEntityByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> SaveEntityAsync(T Entity);
        Task<T> UpdateEntityAsync(T Entity);
        Task<bool> ExistsAsync(int id);
    }
}
