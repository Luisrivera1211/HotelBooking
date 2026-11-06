using HotelBooking.Data.Entities;

namespace HotelBooking.Business.Interfaces
{   
    public interface IHotelService
    {
        Task<Hotel> ActivarHotel();
        Task<Hotel> DesactivarHotel();
        Task<Hotel> AgregarBloque();
    }
}
