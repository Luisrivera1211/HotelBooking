using Microsoft.EntityFrameworkCore;
using HotelBooking.Data.Base;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Context;

namespace HotelBooking.Data.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : AuditEntity
    {
        protected readonly HotelBookingDbContext _context;

        public BaseRepository(HotelBookingDbContext context)
         {
            _context = context;
         }

     public virtual async Task<T?> GetEntityByIdAsync(int id)
        {
           return await _context.Set<T>().FindAsync(id);
        }
     public virtual async Task<List<T>> GetAllAsync()
        {
           return await _context.Set<T>().ToListAsync(); 
        }

     public virtual async Task<T> SaveEntityAsync(T entity)
        {   
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
     public virtual async Task<T> UpdateEntityAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id == id);
        }
    }
}
