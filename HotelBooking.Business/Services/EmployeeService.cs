using HotelBooking.Business.DTOs.Employee;
using HotelBooking.Business.Helpers;
using HotelBooking.Business.Interfaces;
using HotelBooking.Business.Results;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;
using Microsoft.Extensions.Configuration;

namespace HotelBooking.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConfiguration _configuration;

        public EmployeeService(IEmployeeRepository employeeRepository, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _configuration = configuration;
        }

        public async Task<OperationResult<EmployeeDto>> RegisterAsync(RegisterEmployeeDto dto)
        {
            if (dto is null)
                return OperationResult<EmployeeDto>.Validation(DataMessages.EntityRequired);

            var email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty;
            var password = dto.Password ?? string.Empty;

            if (!InputValidator.IsValidEmail(email))
                return OperationResult<EmployeeDto>.Validation(ErrorMessages.InvalidEmail);
            if (password.Length < 8)
                return OperationResult<EmployeeDto>.Validation(ErrorMessages.PasswordTooShort);
            if (password != dto.ConfirmPassword)
                return OperationResult<EmployeeDto>.Validation(ErrorMessages.PasswordMismatch);
            if (!Enum.IsDefined(dto.Role))
                return OperationResult<EmployeeDto>.Validation(ErrorMessages.InvalidRole);

            
            if (dto.Role == EmployeeRole.Administrator)
            {
                var expectedCode = _configuration["AdminRegistrationCode"];
                if (string.IsNullOrWhiteSpace(expectedCode)
                    || !string.Equals(dto.AdminCode, expectedCode, StringComparison.Ordinal))
                    return OperationResult<EmployeeDto>.Validation(ErrorMessages.InvalidAdminCode);
            }

            var exists = await _employeeRepository.ExistsEmailAsync(email);
            if (!exists.Success)
                return OperationResult<EmployeeDto>.Fail(exists.Message, exists.ErrorType);
            if (exists.Data)
                return OperationResult<EmployeeDto>.Conflict(ErrorMessages.EmployeeEmailExists);

            var employee = new Employee
            {
                FullName = dto.FullName?.Trim() ?? string.Empty,
                Email = email,
                PasswordHash = PasswordHasher.Hash(password),
                Role = dto.Role
            };

            var saved = await _employeeRepository.SaveEntityAsync(employee);
            if (!saved.Success)
                return OperationResult<EmployeeDto>.Fail(saved.Message, saved.ErrorType);

            return OperationResult<EmployeeDto>.Ok(ToDto(saved.Data!), saved.Message);
        }

        public async Task<OperationResult<EmployeeDto>> LoginAsync(LoginDto dto)
        {
            if (dto is null
                || string.IsNullOrWhiteSpace(dto.Email)
                || string.IsNullOrEmpty(dto.Password))
                return OperationResult<EmployeeDto>.Validation(ErrorMessages.InvalidCredentials);

            var found = await _employeeRepository.GetByEmailAsync(dto.Email.Trim().ToLowerInvariant());
            if (!found.Success)
                return OperationResult<EmployeeDto>.Fail(found.Message, found.ErrorType);

            
            if (found.Data is null || !PasswordHasher.Verify(dto.Password, found.Data.PasswordHash))
                return OperationResult<EmployeeDto>.Validation(ErrorMessages.InvalidCredentials);

            return OperationResult<EmployeeDto>.Ok(ToDto(found.Data));
        }

        
        private static EmployeeDto ToDto(Employee employee) => new()
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Role = employee.Role
        };
    }
}
