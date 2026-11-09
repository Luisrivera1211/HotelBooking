using HotelBooking.Business.DTOs.Block;
using HotelBooking.Business.Interfaces;
using HotelBooking.Business.Results;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Services
{
    public class BlockService : IBlockService
    {
        protected readonly IBlockRepository _blockRepository;
        protected readonly IHotelRepository _hotelRepository;

        public BlockService(IBlockRepository blockRepository, IHotelRepository hotelRepository)
        {
            _blockRepository = blockRepository;
            _hotelRepository = hotelRepository;
        }

        public async Task<OperationResult<BlockDto>> CreateAsync(CreateBlockDto dto)
        {
            if (dto is null)
                return OperationResult<BlockDto>.Validation(DataMessages.EntityRequired);

            var name = dto.Name?.Trim() ?? string.Empty;

            var hotel = await _hotelRepository.GetEntityByIdAsync(dto.HotelId);
            if (!hotel.Success)
                return OperationResult<BlockDto>.Fail(hotel.Message, hotel.ErrorType);
            if (!hotel.Data!.IsActive)
                return OperationResult<BlockDto>.Conflict(ErrorMessages.HotelInactive);

            var exists = await _blockRepository.ExistsNameInHotelAsync(dto.HotelId, name);
            if (!exists.Success)
                return OperationResult<BlockDto>.Fail(exists.Message, exists.ErrorType);
            if (exists.Data)
                return OperationResult<BlockDto>.Conflict(ErrorMessages.BlockNameExists);

            var block = new Block
            {
                Name = name,
                HotelId = dto.HotelId,
                IsActive = true
            };

            var saved = await _blockRepository.SaveEntityAsync(block);
            if (!saved.Success)
                return OperationResult<BlockDto>.Fail(saved.Message, saved.ErrorType);

            return OperationResult<BlockDto>.Ok(ToDto(saved.Data!), saved.Message);
        }

        public async Task<OperationResult<BlockDto>> GetByIdAsync(int id)
        {
            var found = await _blockRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<BlockDto>.Fail(found.Message, found.ErrorType);

            return OperationResult<BlockDto>.Ok(ToDto(found.Data!));
        }

        public async Task<OperationResult<List<BlockDto>>> GetByHotelAsync(int hotelId)
        {
            var blocks = await _blockRepository.GetByHotelIdAsync(hotelId);
            if (!blocks.Success)
                return OperationResult<List<BlockDto>>.Fail(blocks.Message, blocks.ErrorType);

            return OperationResult<List<BlockDto>>.Ok(blocks.Data!.Select(ToDto).ToList());
        }

        public async Task<OperationResult<BlockDto>> UpdateAsync(int id, CreateBlockDto dto)
        {
            if (dto is null)
                return OperationResult<BlockDto>.Validation(DataMessages.EntityRequired);

            var found = await _blockRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<BlockDto>.Fail(found.Message, found.ErrorType);

            var block = found.Data!;
            var name = dto.Name?.Trim() ?? string.Empty;

            var exists = await _blockRepository.ExistsNameInHotelAsync(block.HotelId, name, id);
            if (!exists.Success)
                return OperationResult<BlockDto>.Fail(exists.Message, exists.ErrorType);
            if (exists.Data)
                return OperationResult<BlockDto>.Conflict(ErrorMessages.BlockNameExists);

            block.Name = name;
            
            var updated = await _blockRepository.UpdateEntityAsync(block);
            if (!updated.Success)
                return OperationResult<BlockDto>.Fail(updated.Message, updated.ErrorType);

            return OperationResult<BlockDto>.Ok(ToDto(updated.Data!), updated.Message);
        }
             

       public async Task<OperationResult<BlockDto>> SetActiveAsync(int id, bool isActive)
        {
            var found = await _blockRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<BlockDto>.Fail(found.Message, found.ErrorType);

            var block = found.Data!;

            if (block.IsActive == isActive)
                return OperationResult<BlockDto>.Ok(ToDto(block));

            block.IsActive = isActive;

            var updated = await _blockRepository.UpdateEntityAsync(block);
            if (!updated.Success)
                return OperationResult<BlockDto>.Fail(updated.Message, updated.ErrorType);

            return OperationResult<BlockDto>.Ok(ToDto(updated.Data!), updated.Message);

        }

        private static BlockDto ToDto(Block block) => new()
        {
            Id = block.Id,
            Name = block.Name,
            IsActive = block.IsActive,
            HotelId = block.HotelId
        };
    }
}
