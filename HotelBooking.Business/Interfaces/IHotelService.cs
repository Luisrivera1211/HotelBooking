using HotelBooking.Business.DTOs.Hotels;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface IHotelService
    {
        Task<OperationResult<HotelDto>> CreateAsync(CreateHotelDto dto);
        Task<OperationResult<HotelDto>> GetByIdAsync(int id);
        Task<OperationResult<List<HotelDto>>> GetAllAsync();
        Task<OperationResult<HotelDto>> UpdateAsync(int id, CreateHotelDto dto);
        Task<OperationResult<HotelDto>> SetActiveAsync(int id, bool isActive);
    }
}
