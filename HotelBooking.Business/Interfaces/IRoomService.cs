using HotelBooking.Business.DTOs.Room;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface IRoomService
    {
        Task<OperationResult<RoomDto>> CreateAsync(CreateRoomDto dto);
        Task<OperationResult<RoomDto>> GetByIdAsync(int id);
        Task<OperationResult<List<RoomDto>>> GetByFloorAsync(int floorId);
        Task<OperationResult<RoomDto>> UpdateAsync(int id, CreateRoomDto dto);
        Task<OperationResult<RoomDto>> ChangeStatusAsync(int id, ChangeRoomStatusDto dto);
    }
}
