using HotelBooking.Business.DTOs.Reservation;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface IReservationService
    {
        Task<OperationResult<ReservationDto>> CreateAsync(CreateReservationDto dto);
        Task<OperationResult<ReservationDto>> GetByIdAsync(int id);
        Task<OperationResult<List<ReservationDto>>> GetByGuestAsync(int guestId);
        Task<OperationResult<ReservationDto>> ModifyAsync(int id, ModifyReservationDto dto);
        Task<OperationResult<ReservationDto>> ConfirmAsync(int id);
        Task<OperationResult<ReservationDto>> CancelAsync(int id);
    }
}
