using HotelBooking.Business.DTOs.RoomType;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface IRoomTypeService
    {
        Task<OperationResult<RoomTypeDto>> CreateAsync(CreateRoomTypeDto dto);
        Task<OperationResult<RoomTypeDto>> GetByIdAsync(int id);
        Task<OperationResult<List<RoomTypeDto>>> GetAllAsync();
        Task<OperationResult<RoomTypeDto>> UpdateAsync(int id, CreateRoomTypeDto dto);
    }
}
