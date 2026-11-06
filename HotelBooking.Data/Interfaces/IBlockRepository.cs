using HotelBooking.Data.Entities;

namespace HotelBooking.Data.Interfaces
{
    public interface IBlockRepository : IBaseRepository<Block>
    {
        Task<List<Block>> GetBlockAsync();

        Task<Block?> GetByNameAsync(string name);

        Task<bool> ExistsNameInHotelAsync(int hotelId, string name, int? excludeBlockId);

    }
}
