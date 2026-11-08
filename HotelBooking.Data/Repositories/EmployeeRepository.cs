// Repositories/EmployeeRepository.cs
using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(HotelBookingDbContext context) : base(context)
        { }

        protected override string? Validate(Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.FullName))
                return DataMessages.Required("Nombre");
            if (employee.FullName.Length > 100)
                return DataMessages.TooLong("Nombre", 100);
            if (string.IsNullOrWhiteSpace(employee.Email))
                return DataMessages.Required("Correo");
            if (employee.Email.Length > 100)
                return DataMessages.TooLong("Correo", 100);
            if (string.IsNullOrWhiteSpace(employee.PasswordHash))
                return DataMessages.Required("Contraseña");
            if (employee.PasswordHash.Length > 255)
                return DataMessages.TooLong("Contraseña", 255);
            return null;
        }

        public async Task<OperationResult<Employee?>> GetByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return OperationResult<Employee?>.Validation(DataMessages.Required("Correo"));

                var employee = await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Email == email.Trim());
                return OperationResult<Employee?>.Ok(employee);
            }
            catch (Exception)
            {
                return OperationResult<Employee?>.Unexpected(DataMessages.QueryError);
            }
        }

        public async Task<OperationResult<bool>> ExistsEmailAsync(string email, int? excludeEmployeeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return OperationResult<bool>.Validation(DataMessages.Required("Correo"));

                var query = _context.Employees.Where(e => e.Email == email.Trim());

                if (excludeEmployeeId.HasValue)
                    query = query.Where(e => e.Id != excludeEmployeeId.Value);

                return OperationResult<bool>.Ok(await query.AnyAsync());
            }
            catch (Exception)
            {
                return OperationResult<bool>.Unexpected(DataMessages.QueryError);
            }
        }
    }
}