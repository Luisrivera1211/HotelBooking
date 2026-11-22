using HotelBooking.Business.DTOs.Employee;
using HotelBooking.Business.DTOs.Guest;
using HotelBooking.Business.Helpers;
using HotelBooking.Business.Interfaces;
using HotelBooking.Business.Results;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Services
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;

        public GuestService(IGuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        public async Task<OperationResult<GuestDto>> CreateAsync(CreateGuestDto dto)
        {
            if (dto is null)
                return OperationResult<GuestDto>.Validation(DataMessages.EntityRequired);

            var email = dto.Email?.Trim() ?? string.Empty;
            var document = dto.Document?.Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(email) && !InputValidator.IsValidEmail(email))
                return OperationResult<GuestDto>.Validation(ErrorMessages.InvalidEmail);

            var exists = await _guestRepository.ExistsDocumentAsync(document);
            if (!exists.Success)
                return OperationResult<GuestDto>.Fail(exists.Message, exists.ErrorType);
            if (exists.Data)
                return OperationResult<GuestDto>.Conflict(ErrorMessages.GuestDocumentExists);

            var guest = new Guest
            {
                FullName = dto.FullName?.Trim() ?? string.Empty,
                Document = document,
                Email = email,
                Phone = dto.Phone?.Trim() ?? string.Empty
            };

            var saved = await _guestRepository.SaveEntityAsync(guest);
            if (!saved.Success)
                return OperationResult<GuestDto>.Fail(saved.Message, saved.ErrorType);

            return OperationResult<GuestDto>.Ok(ToDto(saved.Data!), saved.Message);
        }

        public async Task<OperationResult<GuestDto>> GetByIdAsync(int id)
        {
            var found = await _guestRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<GuestDto>.Fail(found.Message, found.ErrorType);

            return OperationResult<GuestDto>.Ok(ToDto(found.Data!));
        }
        public async Task<OperationResult<GuestDto>> GetByDocumentAsync(string document)
        {
            var found = await _guestRepository.GetByDocumentAsync(document);
            if (!found.Success)
                return OperationResult<GuestDto>.Fail(found.Message, found.ErrorType);
            if (found.Data is null)
                return OperationResult<GuestDto>.NotFound(ErrorMessages.GuestNotFoundByDocument);

            return OperationResult<GuestDto>.Ok(ToDto(found.Data));
        }

        public async Task<OperationResult<List<GuestDto>>> GetAllAsync()
        {
            var all = await _guestRepository.GetAllAsync();
            if (!all.Success)
                return OperationResult<List<GuestDto>>.Fail(all.Message, all.ErrorType);

            return OperationResult<List<GuestDto>>.Ok(all.Data!.Select(ToDto).ToList());
        }

        public async Task<OperationResult<GuestDto>> UpdateAsync(int id, CreateGuestDto dto)
        {
            if (dto is null)
                return OperationResult<GuestDto>.Validation(DataMessages.EntityRequired);

            var found = await _guestRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<GuestDto>.Fail(found.Message, found.ErrorType);

            var email = dto.Email?.Trim() ?? string.Empty;
            var document = dto.Document?.Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(email) && !InputValidator.IsValidEmail(email))
                return OperationResult<GuestDto>.Validation(ErrorMessages.InvalidEmail);

            var exists = await _guestRepository.ExistsDocumentAsync(document, id);
            if (!exists.Success)
                return OperationResult<GuestDto>.Fail(exists.Message, exists.ErrorType);
            if (exists.Data)
                return OperationResult<GuestDto>.Conflict(ErrorMessages.GuestDocumentExists);

            var guest = found.Data!;
            guest.FullName = dto.FullName?.Trim() ?? string.Empty;
            guest.Document = document;
            guest.Email = email;
            guest.Phone = dto.Phone?.Trim() ?? string.Empty;

            var updated = await _guestRepository.UpdateEntityAsync(guest);
            if (!updated.Success)
                return OperationResult<GuestDto>.Fail(updated.Message, updated.ErrorType);

            return OperationResult<GuestDto>.Ok(ToDto(updated.Data!), updated.Message);
        }
        public async Task<OperationResult<GuestDto>> RegisterAccountAsync(RegisterGuestAccountDto dto)
        {
            if (dto is null)
                return OperationResult<GuestDto>.Validation(DataMessages.EntityRequired);

            var email = dto.Email?.Trim().ToLowerInvariant() ?? string.Empty;
            var document = dto.Document?.Trim() ?? string.Empty;
            var password = dto.Password ?? string.Empty;

            if (!InputValidator.IsValidEmail(email))
                return OperationResult<GuestDto>.Validation(ErrorMessages.InvalidEmail);
            if (password.Length < 8)
                return OperationResult<GuestDto>.Validation(ErrorMessages.PasswordTooShort);
            if (password != dto.ConfirmPassword)
                return OperationResult<GuestDto>.Validation(ErrorMessages.PasswordMismatch);

            var withAccount = await _guestRepository.GetByEmailWithAccountAsync(email);
            if (!withAccount.Success)
                return OperationResult<GuestDto>.Fail(withAccount.Message, withAccount.ErrorType);
            if (withAccount.Data is not null)
                return OperationResult<GuestDto>.Conflict(ErrorMessages.GuestAccountExists);

            var byDocument = await _guestRepository.GetByDocumentAsync(document);
            if (!byDocument.Success)
                return OperationResult<GuestDto>.Fail(byDocument.Message, byDocument.ErrorType);

            Guest guest;
            if (byDocument.Data is null)
            {
                guest = new Guest
                {
                    FullName = dto.FullName?.Trim() ?? string.Empty,
                    Document = document,
                    Email = email,
                    Phone = dto.Phone?.Trim() ?? string.Empty,
                    PasswordHash = PasswordHasher.Hash(password)
                };

                var created = await _guestRepository.SaveEntityAsync(guest);
                if (!created.Success)
                    return OperationResult<GuestDto>.Fail(created.Message, created.ErrorType);

                return OperationResult<GuestDto>.Ok(ToDto(created.Data!), created.Message);
            }

            guest = byDocument.Data;
            if (guest.PasswordHash is not null)
                return OperationResult<GuestDto>.Conflict(ErrorMessages.GuestDocumentHasAccount);
            if (!string.Equals(guest.Email, email, StringComparison.OrdinalIgnoreCase))
                return OperationResult<GuestDto>.Conflict(ErrorMessages.GuestDocumentOtherEmail);

            guest.PasswordHash = PasswordHasher.Hash(password);

            var linked = await _guestRepository.UpdateEntityAsync(guest);
            if (!linked.Success)
                return OperationResult<GuestDto>.Fail(linked.Message, linked.ErrorType);

            return OperationResult<GuestDto>.Ok(ToDto(linked.Data!), linked.Message);
        }

        public async Task<OperationResult<GuestDto>> LoginAsync(LoginDto dto)
        {
            if (dto is null
                || string.IsNullOrWhiteSpace(dto.Email)
                || string.IsNullOrEmpty(dto.Password))
                return OperationResult<GuestDto>.Validation(ErrorMessages.InvalidCredentials);

            var found = await _guestRepository.GetByEmailWithAccountAsync(dto.Email);
            if (!found.Success)
                return OperationResult<GuestDto>.Fail(found.Message, found.ErrorType);

            if (found.Data is null || !PasswordHasher.Verify(dto.Password, found.Data.PasswordHash!))
                return OperationResult<GuestDto>.Validation(ErrorMessages.InvalidCredentials);

            return OperationResult<GuestDto>.Ok(ToDto(found.Data));
        }

        private static GuestDto ToDto(Guest guest) => new()
        {
            Id = guest.Id,
            FullName = guest.FullName,
            Document = guest.Document,
            Email = guest.Email,
            Phone = guest.Phone
        };
    }
}
