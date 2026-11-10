using HotelBooking.Business.DTOs.Availability;
using HotelBooking.Business.DTOs.Room;
using HotelBooking.Business.Interfaces;
using HotelBooking.Business.Mappers;
using HotelBooking.Business.Results;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IHotelRepository _hotelRepository;

        public AvailabilityService(IRoomRepository roomRepository, IHotelRepository hotelRepository)
        {
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
        }

        // CU-005
        public async Task<OperationResult<List<RoomDto>>> SearchAsync(AvailabilitySearchDto dto)
        {
            if (dto is null)
                return OperationResult<List<RoomDto>>.Validation(DataMessages.EntityRequired);

            var checkIn = dto.CheckIn.Date;
            var checkOut = dto.CheckOut.Date;

            if (checkOut <= checkIn)
                return OperationResult<List<RoomDto>>.Validation(ErrorMessages.InvalidDates);
            if (checkIn < DateTime.Today)
                return OperationResult<List<RoomDto>>.Validation(ErrorMessages.PastDates);
            if (dto.GuestCount <= 0)
                return OperationResult<List<RoomDto>>.Validation(DataMessages.MustBePositive("Cantidad de huéspedes"));

            // A3: un hotel inactivo no ofrece habitaciones
            var hotel = await _hotelRepository.GetEntityByIdAsync(dto.HotelId);
            if (!hotel.Success)
                return OperationResult<List<RoomDto>>.Fail(hotel.Message, hotel.ErrorType);
            if (!hotel.Data!.IsActive)
                return OperationResult<List<RoomDto>>.Conflict(ErrorMessages.HotelInactive);

            var rooms = await _roomRepository.GetAvailableAsync(dto.HotelId, checkIn, checkOut, dto.GuestCount);
            if (!rooms.Success)
                return OperationResult<List<RoomDto>>.Fail(rooms.Message, rooms.ErrorType);

            // GetAvailableAsync ya trae el RoomType cargado
            var list = rooms.Data!.Select(r => RoomMapper.ToDto(r, r.RoomType)).ToList();

            // A1: sin resultados no es un error, pero la pantalla debe poder decirlo
            return OperationResult<List<RoomDto>>.Ok(list, list.Count == 0 ? ErrorMessages.NoRoomsAvailable : string.Empty);
        }
    }
}
