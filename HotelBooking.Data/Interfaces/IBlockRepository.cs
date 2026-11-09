using HotelBooking.Data.Entities;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Interfaces
{
    public interface IBlockRepository : IBaseRepository<Block>
    {
        Task<OperationResult<List<Block>>> GetByHotelIdAsync(int hotelId);
        Task<OperationResult<bool>> ExistsNameInHotelAsync(int hotelId, string name, int? excludeBlockId = null);

    }
}
