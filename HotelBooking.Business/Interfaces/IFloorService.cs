using HotelBooking.Data.Entities;

namespace HotelBooking.Business.Interfaces
{
    public interface IFloorService
    {
        Task<Floor> ActivarPiso();
        Task<Floor> DesacrivarPiso();
        Task<Floor> AgregarHabitacion();
        Task<Floor> ValidarNumeroUnico();
    }
}
