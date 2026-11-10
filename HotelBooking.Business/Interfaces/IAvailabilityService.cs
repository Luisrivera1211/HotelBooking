using HotelBooking.Business.DTOs.Availability;
using HotelBooking.Business.DTOs.Room;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Interfaces
{
    public interface IAvailabilityService
    {
        Task<OperationResult<List<RoomDto>>> SearchAsync(AvailabilitySearchDto dto);
    }
}
