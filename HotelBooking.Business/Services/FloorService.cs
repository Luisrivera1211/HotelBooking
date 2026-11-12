using HotelBooking.Business.DTOs.Floor;
using HotelBooking.Business.Interfaces;
using HotelBooking.Business.Results;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Services
{
    public class FloorService : IFloorService
    {
        private readonly IFloorRepository _floorRepository;
        private readonly IBlockRepository _blockRepository;

        public FloorService(IFloorRepository floorRepository, IBlockRepository blockRepository)
        {
            _floorRepository = floorRepository;
            _blockRepository = blockRepository;
        }

        public async Task<OperationResult<FloorDto>> CreateAsync(CreateFloorDto dto)
        {
            if (dto is null)
                return OperationResult<FloorDto>.Validation(DataMessages.EntityRequired);

            var block = await _blockRepository.GetEntityByIdAsync(dto.BlockId);
            if (!block.Success)
                return OperationResult<FloorDto>.Fail(block.Message, block.ErrorType);
            if (!block.Data!.IsActive)
                return OperationResult<FloorDto>.Conflict(ErrorMessages.BlockInactive);

            var exists = await _floorRepository.ExistsActiveNumberAsync(dto.BlockId, dto.Number);
            if (!exists.Success)
                return OperationResult<FloorDto>.Fail(exists.Message, exists.ErrorType);
            if (exists.Data)
                return OperationResult<FloorDto>.Conflict(string.Format(ErrorMessages.FloorNumberExists, dto.Number));

            var floor = new Floor
            {
                Number = dto.Number,
                BlockId = dto.BlockId,
                IsActive = true
            };

            var saved = await _floorRepository.SaveEntityAsync(floor);
            if (!saved.Success)
                return OperationResult<FloorDto>.Fail(saved.Message, saved.ErrorType);

            return OperationResult<FloorDto>.Ok(ToDto(saved.Data!), saved.Message);
        }

        public async Task<OperationResult<FloorDto>> GetByIdAsync(int id)
        {
            var found = await _floorRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<FloorDto>.Fail(found.Message, found.ErrorType);

            return OperationResult<FloorDto>.Ok(ToDto(found.Data!));
        }

        public async Task<OperationResult<List<FloorDto>>> GetByBlockAsync(int blockId)
        {
            var floors = await _floorRepository.GetByBlockIdAsync(blockId);
            if (!floors.Success)
                return OperationResult<List<FloorDto>>.Fail(floors.Message, floors.ErrorType);

            return OperationResult<List<FloorDto>>.Ok(floors.Data!.Select(ToDto).ToList());
        }

        public async Task<OperationResult<FloorDto>> SetActiveAsync(int id, bool isActive)
        {
            var found = await _floorRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<FloorDto>.Fail(found.Message, found.ErrorType);

            var floor = found.Data!;

            
            if (floor.IsActive == isActive)
                return OperationResult<FloorDto>.Ok(ToDto(floor));

            if (isActive)
            {
                var exists = await _floorRepository.ExistsActiveNumberAsync(floor.BlockId, floor.Number, id);
                if (!exists.Success)
                    return OperationResult<FloorDto>.Fail(exists.Message, exists.ErrorType);
                if (exists.Data)
                    return OperationResult<FloorDto>.Conflict(string.Format(ErrorMessages.FloorNumberExists, floor.Number));
            }

            floor.IsActive = isActive;

            var updated = await _floorRepository.UpdateEntityAsync(floor);
            if (!updated.Success)
                return OperationResult<FloorDto>.Fail(updated.Message, updated.ErrorType);

            return OperationResult<FloorDto>.Ok(ToDto(updated.Data!), updated.Message);
        }

        private static FloorDto ToDto(Floor floor) => new()
        {
            Id = floor.Id,
            Number = floor.Number,
            IsActive = floor.IsActive,
            BlockId = floor.BlockId
        };
    }
}
