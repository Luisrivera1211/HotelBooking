using HotelBooking.Business.DTOs.Guest;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface IGuestService
    {
        Task<OperationResult<GuestDto>> CreateAsync(CreateGuestDto dto);
        Task<OperationResult<GuestDto>> GetByIdAsync(int id);
        Task<OperationResult<GuestDto>> GetByDocumentAsync(string document);
        Task<OperationResult<List<GuestDto>>> GetAllAsync();
        Task<OperationResult<GuestDto>> UpdateAsync(int id, CreateGuestDto dto);
    }
}
