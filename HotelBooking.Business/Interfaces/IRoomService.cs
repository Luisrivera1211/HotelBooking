using HotelBooking.Data.Entities;

namespace HotelBooking.Business.Interfaces
{
    public interface IRoomService
    {
        Task<Room> CambiarEstado();
        Task<Room> VerificarDisponibilidad();
        Task<Room> ValidarCapacidad();
    }
}
