using HotelBooking.Business.DTOs.Floor;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface IFloorService
    {
        Task<OperationResult<FloorDto>> CreateAsync(CreateFloorDto dto);
        Task<OperationResult<FloorDto>> GetByIdAsync(int id);
        Task<OperationResult<List<FloorDto>>> GetByBlockAsync(int blockId);
        Task<OperationResult<FloorDto>> SetActiveAsync(int id, bool isActive);
    }
}
