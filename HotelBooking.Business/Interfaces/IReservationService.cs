using HotelBooking.Data.Entities;

namespace HotelBooking.Business.Interfaces
{
    public interface IReservationService
    {
        Task<Reservation> CreateReservation();
        Task<Reservation> ConfirmReservation();
        Task<Reservation> UpdateReservation();
        Task<Reservation> CancelReservation();
        Task<Reservation> TotalReservation();
        Task<Reservation> ValidateDatesReservation();
        Task<Reservation> ValidateSolapamiento();
    }
}
