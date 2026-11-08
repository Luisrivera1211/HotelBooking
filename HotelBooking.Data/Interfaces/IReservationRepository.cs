using HotelBooking.Data.Entities;
using HotelBooking.Data.Results;


namespace HotelBooking.Data.Interfaces
{
    public interface IReservationRepository : IBaseRepository<Reservation>
    {
        Task<OperationResult<List<Reservation>>> GetOverlappingAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeReservationId = null);

        Task<OperationResult<List<Reservation>>> GetByGuestAsync(int guestId);

        Task<OperationResult<Reservation?>> GetWithDetailsAsync(int id);
    }
}
