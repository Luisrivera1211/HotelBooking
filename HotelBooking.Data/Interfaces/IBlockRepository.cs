using HotelBooking.Data.Entities;
using HotelBooking.Data.Results;

namespace HotelBooking.Data.Interfaces
{
    public interface IBlockRepository : IBaseRepository<Block>
    {
        Task<OperationResult<List<Block>>> GetByHotelIdAsync(int hotelId);
        Task<OperationResult<Block?>> GetByNameAsync(int hotelId, string name);
        Task<OperationResult<bool>> ExistsNameInHotelAsync(int hotelId, string name, int? excludeBlockId = null);

    }
}
