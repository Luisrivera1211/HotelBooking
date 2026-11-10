using HotelBooking.Business.DTOs.Room;
using HotelBooking.Business.Interfaces;
using HotelBooking.Business.Mappers;
using HotelBooking.Business.Results;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IBaseRepository<RoomType> _roomTypeRepository;

        public RoomService(
            IRoomRepository roomRepository,
            IFloorRepository floorRepository,
            IBlockRepository blockRepository,
            IBaseRepository<RoomType> roomTypeRepository)
        {
            _roomRepository = roomRepository;
            _floorRepository = floorRepository;
            _blockRepository = blockRepository;
            _roomTypeRepository = roomTypeRepository;
        }

        public async Task<OperationResult<RoomDto>> CreateAsync(CreateRoomDto dto)
        {
            if (dto is null)
                return OperationResult<RoomDto>.Validation(DataMessages.EntityRequired);

            var number = dto.Number?.Trim() ?? string.Empty;

            var check = await ValidateAsync(dto.FloorId, dto.RoomTypeId, number, null);
            if (!check.Success)
                return OperationResult<RoomDto>.Fail(check.Message, check.ErrorType);

            var room = new Room
            {
                FloorId = dto.FloorId,
                RoomTypeId = dto.RoomTypeId,
                Number = number,
                Status = RoomStatus.Available   // el estado inicial lo decide el servicio
            };

            var saved = await _roomRepository.SaveEntityAsync(room);
            if (!saved.Success)
                return OperationResult<RoomDto>.Fail(saved.Message, saved.ErrorType);

            return OperationResult<RoomDto>.Ok(RoomMapper.ToDto(saved.Data!, check.Data!), saved.Message);
        }

        public async Task<OperationResult<RoomDto>> GetByIdAsync(int id)
        {
            var found = await _roomRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<RoomDto>.Fail(found.Message, found.ErrorType);

            return await ToDtoAsync(found.Data!, string.Empty);
        }

        public async Task<OperationResult<List<RoomDto>>> GetByFloorAsync(int floorId)
        {
            var rooms = await _roomRepository.GetByFloorAsync(floorId);
            if (!rooms.Success)
                return OperationResult<List<RoomDto>>.Fail(rooms.Message, rooms.ErrorType);

            // Los tipos se cargan una sola vez para no consultar uno por habitación
            var types = await _roomTypeRepository.GetAllAsync();
            if (!types.Success)
                return OperationResult<List<RoomDto>>.Fail(types.Message, types.ErrorType);

            var typeById = types.Data!.ToDictionary(t => t.Id);
            var list = rooms.Data!
                .Select(r => RoomMapper.ToDto(r, typeById[r.RoomTypeId]))
                .ToList();

            return OperationResult<List<RoomDto>>.Ok(list);
        }

        public async Task<OperationResult<RoomDto>> UpdateAsync(int id, CreateRoomDto dto)
        {
            if (dto is null)
                return OperationResult<RoomDto>.Validation(DataMessages.EntityRequired);

            var found = await _roomRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<RoomDto>.Fail(found.Message, found.ErrorType);

            var number = dto.Number?.Trim() ?? string.Empty;

            // Se excluye la propia habitación para poder guardar sin cambiar el número
            var check = await ValidateAsync(dto.FloorId, dto.RoomTypeId, number, id);
            if (!check.Success)
                return OperationResult<RoomDto>.Fail(check.Message, check.ErrorType);

            var room = found.Data!;
            room.FloorId = dto.FloorId;
            room.RoomTypeId = dto.RoomTypeId;
            room.Number = number;

            var updated = await _roomRepository.UpdateEntityAsync(room);
            if (!updated.Success)
                return OperationResult<RoomDto>.Fail(updated.Message, updated.ErrorType);

            return OperationResult<RoomDto>.Ok(RoomMapper.ToDto(updated.Data!, check.Data!), updated.Message);
        }

        // RF-007
        public async Task<OperationResult<RoomDto>> ChangeStatusAsync(int id, ChangeRoomStatusDto dto)
        {
            if (dto is null)
                return OperationResult<RoomDto>.Validation(DataMessages.EntityRequired);
            if (!Enum.IsDefined(dto.Status))
                return OperationResult<RoomDto>.Validation(ErrorMessages.InvalidRoomStatus);

            var found = await _roomRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<RoomDto>.Fail(found.Message, found.ErrorType);

            var room = found.Data!;

            if (room.Status != dto.Status)
            {
                room.Status = dto.Status;

                var updated = await _roomRepository.UpdateEntityAsync(room);
                if (!updated.Success)
                    return OperationResult<RoomDto>.Fail(updated.Message, updated.ErrorType);

                room = updated.Data!;
            }

            return await ToDtoAsync(room, string.Empty);
        }

        // Comprueba piso (existe y activo), tipo (existe) y número único en el hotel.
        // Devuelve el tipo de habitación para armar el DTO sin otra consulta.
        private async Task<OperationResult<RoomType>> ValidateAsync(int floorId, int roomTypeId, string number, int? excludeRoomId)
        {
            var floor = await _floorRepository.GetEntityByIdAsync(floorId);
            if (!floor.Success)
                return OperationResult<RoomType>.Fail(floor.Message, floor.ErrorType);
            if (!floor.Data!.IsActive)
                return OperationResult<RoomType>.Conflict(ErrorMessages.FloorInactive);

            var type = await _roomTypeRepository.GetEntityByIdAsync(roomTypeId);
            if (!type.Success)
                return OperationResult<RoomType>.Fail(type.Message, type.ErrorType);

            // Room solo apunta a Floor: el hotel se alcanza por Floor -> Block -> Hotel
            var block = await _blockRepository.GetEntityByIdAsync(floor.Data.BlockId);
            if (!block.Success)
                return OperationResult<RoomType>.Fail(block.Message, block.ErrorType);

            // RN04: el número es único dentro del hotel
            var exists = await _roomRepository.ExistsNumberInHotelAsync(block.Data!.HotelId, number, excludeRoomId);
            if (!exists.Success)
                return OperationResult<RoomType>.Fail(exists.Message, exists.ErrorType);
            if (exists.Data)
                return OperationResult<RoomType>.Conflict(string.Format(ErrorMessages.RoomNumberExists, number));

            return OperationResult<RoomType>.Ok(type.Data);
        }

        private async Task<OperationResult<RoomDto>> ToDtoAsync(Room room, string message)
        {
            var type = await _roomTypeRepository.GetEntityByIdAsync(room.RoomTypeId);
            if (!type.Success)
                return OperationResult<RoomDto>.Fail(type.Message, type.ErrorType);

            return OperationResult<RoomDto>.Ok(RoomMapper.ToDto(room, type.Data!), message);
        }
    }
}
