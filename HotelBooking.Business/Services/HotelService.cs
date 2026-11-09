using HotelBooking.Business.DTOs.Hotels;
using HotelBooking.Business.Interfaces;
using HotelBooking.Business.Results;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<OperationResult<HotelDto>> CreateAsync(CreateHotelDto dto)
        {
            if (dto is null)
                return OperationResult<HotelDto>.Validation(DataMessages.EntityRequired);

            var name = dto.Name?.Trim() ?? string.Empty;

            var existing = await _hotelRepository.GetByNameAsync(name);
            if (!existing.Success)
                return OperationResult<HotelDto>.Fail(existing.Message, existing.ErrorType);
            if (existing.Data is not null)
                return OperationResult<HotelDto>.Conflict(ErrorMessages.HotelNameExists);

            var hotel = new Hotel
            {
                Name = name,
                Address = dto.Address?.Trim() ?? string.Empty,
                IsActive = true
            };

            var saved = await _hotelRepository.SaveEntityAsync(hotel);
            if (!saved.Success)
                return OperationResult<HotelDto>.Fail(saved.Message, saved.ErrorType);

            return OperationResult<HotelDto>.Ok(ToDto(saved.Data!), saved.Message);
        }

        public async Task<OperationResult<HotelDto>> GetByIdAsync(int id)
        {
            var found = await _hotelRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<HotelDto>.Fail(found.Message, found.ErrorType);

            return OperationResult<HotelDto>.Ok(ToDto(found.Data!));
        }

        public async Task<OperationResult<List<HotelDto>>> GetAllAsync()
        {
            var all = await _hotelRepository.GetAllAsync();
            if (!all.Success)
                return OperationResult<List<HotelDto>>.Fail(all.Message, all.ErrorType);

            return OperationResult<List<HotelDto>>.Ok(all.Data!.Select(ToDto).ToList());
        }

        public async Task<OperationResult<HotelDto>> UpdateAsync(int id, CreateHotelDto dto)
        {
            if (dto is null)
                return OperationResult<HotelDto>.Validation(DataMessages.EntityRequired);

            var found = await _hotelRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<HotelDto>.Fail(found.Message, found.ErrorType);

            var name = dto.Name?.Trim() ?? string.Empty;

            var sameName = await _hotelRepository.GetByNameAsync(name);
            if (!sameName.Success)
                return OperationResult<HotelDto>.Fail(sameName.Message, sameName.ErrorType);
            if (sameName.Data is not null && sameName.Data.Id != id)
                return OperationResult<HotelDto>.Conflict(ErrorMessages.HotelNameExists);

            var hotel = found.Data!;
            hotel.Name = name;
            hotel.Address = dto.Address?.Trim() ?? string.Empty;

            var updated = await _hotelRepository.UpdateEntityAsync(hotel);
            if (!updated.Success)
                return OperationResult<HotelDto>.Fail(updated.Message, updated.ErrorType);

            return OperationResult<HotelDto>.Ok(ToDto(updated.Data!), updated.Message);
        }

        public async Task<OperationResult<HotelDto>> SetActiveAsync(int id, bool isActive)
        {
            var found = await _hotelRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<HotelDto>.Fail(found.Message, found.ErrorType);

            var hotel = found.Data!;

            if (hotel.IsActive == isActive)
                return OperationResult<HotelDto>.Ok(ToDto(hotel));

            hotel.IsActive = isActive;

            var updated = await _hotelRepository.UpdateEntityAsync(hotel);
            if (!updated.Success)
                return OperationResult<HotelDto>.Fail(updated.Message, updated.ErrorType);

            return OperationResult<HotelDto>.Ok(ToDto(updated.Data!), updated.Message);
        }

        private static HotelDto ToDto(Hotel hotel) => new()
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            IsActive = hotel.IsActive
        };
    }
}
