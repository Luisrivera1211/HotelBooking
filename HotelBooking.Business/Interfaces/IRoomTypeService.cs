using HotelBooking.Data.Entities;

namespace HotelBooking.Business.Interfaces
{
    public interface IRoomTypeService
    {
        Task<RoomType> ObtenerTarifaAplicable(int RoomId);

    }
}
