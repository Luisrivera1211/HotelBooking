using Microsoft.EntityFrameworkCore;
using HotelBooking.Data.Base;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Context;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : AuditEntity
    {
        protected readonly HotelBookingDbContext _context;

        public BaseRepository(HotelBookingDbContext context)
         {
            _context = context;
         }

     protected virtual string? Validate(T entity) => null;

        protected static OperationResult<TResult> Failure<TResult>(string message) =>
               new() { Success = false, Message = message };

     public async Task<OperationResult<T?>> GetEntityByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<T>.Validation(DataMessages.InvalidId);

                var entity = await _context.Set<T>().FindAsync(id);
                return entity is null
                ? OperationResult<T>.NotFound(DataMessages.NotFound)
                : OperationResult<T>.Ok(entity);

            }
            catch (Exception)
            {
                return OperationResult<T>.Unexpected(DataMessages.QueryError);
            }
        }

     public async Task<OperationResult<List<T>>> GetAllAsync()
        {
            try
            {
                var list = await _context.Set<T>().AsNoTracking().ToListAsync();
                return OperationResult<List<T>>.Ok(list);
            }
            catch (Exception)
            {
                return OperationResult<List<T>>.Unexpected(DataMessages.QueryError);
            }
        }

        public virtual async Task<OperationResult<T>> SaveEntityAsync(T entity)
        {
            try
            {
                if (entity is null)
                    return OperationResult<T>.Validation(DataMessages.EntityRequired);

                var error = Validate(entity);
                if (error is not null)
                    return OperationResult<T>.Validation(error);

                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return OperationResult<T>.Ok(entity, "Registro guardado correctamente.");
            }
            catch (DbUpdateException)
            {
                return OperationResult<T>.Conflict(DataMessages.ConstraintViolation);
            }
            catch (Exception)
            {
                return OperationResult<T>.Unexpected(DataMessages.SaveError);
            }
        }

     public virtual async Task<OperationResult<T>> UpdateEntityAsync(T entity)
        {
            try
            {
                if (entity is null)
                    return OperationResult<T>.Validation(DataMessages.EntityRequired);
                if (entity.Id <= 0)
                    return OperationResult<T>.Validation(DataMessages.InvalidId);


                var error = Validate(entity);
                if (error is not null)
                    return OperationResult<T>.Validation(error);

                if (!await _context.Set<T>().AnyAsync(e => e.Id == entity.Id))
                    return OperationResult<T>.NotFound(DataMessages.NotFound);

                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
                return OperationResult<T>.Ok(entity, "Registro actualizado correctamente.");
            }
            catch (DbUpdateException)
            {
                return OperationResult<T>.Conflict(DataMessages.ConstraintViolation);
            }
            catch (Exception)
            {
                return OperationResult<T>.Unexpected(DataMessages.UpdateError);
            }
        }

        public async Task<OperationResult<bool>> ExistsAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<bool>.Validation(DataMessages.InvalidId);

                var exists = await _context.Set<T>().AnyAsync(e => e.Id == id);
                return OperationResult<bool>.Ok(exists);
            }
            catch (Exception)
            {
                return OperationResult<bool>.Unexpected(DataMessages.QueryError);
            }
        }
    }
}
