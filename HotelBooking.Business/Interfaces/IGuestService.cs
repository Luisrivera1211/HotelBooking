using HotelBooking.Data.Entities;

namespace HotelBooking.Business.Interfaces
{
   public interface IGuestService
    {
        Task<Guest> ActualizarDatos();
        Task<Guest> ConsultarHistorial();
    }
}
