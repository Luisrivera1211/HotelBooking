using HotelBooking.Business.DTOs.RoomType;
using HotelBooking.Business.Interfaces;
using HotelBooking.Business.Results;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Services
{
    // RoomType no tiene repositorio propio: usa el genérico. Por eso no existe un Validate en Data
    // para esta entidad y los campos se validan aquí.
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IBaseRepository<RoomType> _roomTypeRepository;

        public RoomTypeService(IBaseRepository<RoomType> roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }

        public async Task<OperationResult<RoomTypeDto>> CreateAsync(CreateRoomTypeDto dto)
        {
            if (dto is null)
                return OperationResult<RoomTypeDto>.Validation(DataMessages.EntityRequired);

            var name = dto.Name?.Trim() ?? string.Empty;

            var fieldError = ValidateFields(name, dto.Capacity, dto.RatePerNight);
            if (fieldError is not null)
                return OperationResult<RoomTypeDto>.Validation(fieldError);

            var exists = await NameExistsAsync(name, null);
            if (!exists.Success)
                return OperationResult<RoomTypeDto>.Fail(exists.Message, exists.ErrorType);
            if (exists.Data)
                return OperationResult<RoomTypeDto>.Conflict(ErrorMessages.RoomTypeNameExists);

            var roomType = new RoomType
            {
                Name = name,
                Capacity = dto.Capacity,
                RatePerNight = dto.RatePerNight
            };

            var saved = await _roomTypeRepository.SaveEntityAsync(roomType);
            if (!saved.Success)
                return OperationResult<RoomTypeDto>.Fail(saved.Message, saved.ErrorType);

            return OperationResult<RoomTypeDto>.Ok(ToDto(saved.Data!), saved.Message);
        }

        public async Task<OperationResult<RoomTypeDto>> GetByIdAsync(int id)
        {
            var found = await _roomTypeRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<RoomTypeDto>.Fail(found.Message, found.ErrorType);

            return OperationResult<RoomTypeDto>.Ok(ToDto(found.Data!));
        }

        public async Task<OperationResult<List<RoomTypeDto>>> GetAllAsync()
        {
            var all = await _roomTypeRepository.GetAllAsync();
            if (!all.Success)
                return OperationResult<List<RoomTypeDto>>.Fail(all.Message, all.ErrorType);

            return OperationResult<List<RoomTypeDto>>.Ok(all.Data!.Select(ToDto).ToList());
        }

        public async Task<OperationResult<RoomTypeDto>> UpdateAsync(int id, CreateRoomTypeDto dto)
        {
            if (dto is null)
                return OperationResult<RoomTypeDto>.Validation(DataMessages.EntityRequired);

            var found = await _roomTypeRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<RoomTypeDto>.Fail(found.Message, found.ErrorType);

            var name = dto.Name?.Trim() ?? string.Empty;

            var fieldError = ValidateFields(name, dto.Capacity, dto.RatePerNight);
            if (fieldError is not null)
                return OperationResult<RoomTypeDto>.Validation(fieldError);

            // Se excluye el propio tipo para poder guardar sin cambiar el nombre
            var exists = await NameExistsAsync(name, id);
            if (!exists.Success)
                return OperationResult<RoomTypeDto>.Fail(exists.Message, exists.ErrorType);
            if (exists.Data)
                return OperationResult<RoomTypeDto>.Conflict(ErrorMessages.RoomTypeNameExists);

            var roomType = found.Data!;
            roomType.Name = name;
            roomType.Capacity = dto.Capacity;
            roomType.RatePerNight = dto.RatePerNight;

            var updated = await _roomTypeRepository.UpdateEntityAsync(roomType);
            if (!updated.Success)
                return OperationResult<RoomTypeDto>.Fail(updated.Message, updated.ErrorType);

            return OperationResult<RoomTypeDto>.Ok(ToDto(updated.Data!), updated.Message);
        }

        private static string? ValidateFields(string name, int capacity, decimal ratePerNight)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DataMessages.Required("Nombre");
            if (name.Length > 50)
                return DataMessages.TooLong("Nombre", 50);
            if (capacity <= 0)
                return DataMessages.MustBePositive("Capacidad");
            if (ratePerNight < 0)
                return "La tarifa por noche no puede ser negativa.";
            return null;
        }

        private async Task<OperationResult<bool>> NameExistsAsync(string name, int? excludeId)
        {
            var all = await _roomTypeRepository.GetAllAsync();
            if (!all.Success)
                return OperationResult<bool>.Fail(all.Message, all.ErrorType);

            var exists = all.Data!.Any(t =>
                t.Id != excludeId && string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
            return OperationResult<bool>.Ok(exists);
        }

        private static RoomTypeDto ToDto(RoomType type) => new()
        {
            Id = type.Id,
            Name = type.Name,
            Capacity = type.Capacity,
            RatePerNight = type.RatePerNight
        };
    }
}
