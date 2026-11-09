using HotelBooking.Business.DTOs.Block;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface IBlockService
    {
        Task<OperationResult<BlockDto>> CreateAsync(CreateBlockDto dto);

        Task<OperationResult<BlockDto>> GetByIdAsync(int id);

        Task<OperationResult<List<BlockDto>>> GetAllAsync(int hotelId);

        Task<OperationResult<BlockDto>> UpdateAsync(int id, CreateBlockDto dto);

        Task<OperationResult<BlockDto>> SetActiveAsync(int id, bool isActive);

    }
}
