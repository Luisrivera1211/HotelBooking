using HotelBooking.Data.Entities;


namespace HotelBooking.Data.Interfaces
{
    public interface IReservationRepository : IBaseRepository<Reservation>
    {
        Task<List<Reservation>> GetOverlappingAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeReservationId = null);

        Task<List<Reservation>> GetByGuestAsync(int guestId);

        Task<Reservation?> GetWithDetailsAsync(int id);
    }
}
