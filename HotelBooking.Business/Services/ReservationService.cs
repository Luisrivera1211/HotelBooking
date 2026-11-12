using HotelBooking.Business.DTOs.Reservation;
using HotelBooking.Business.Interfaces;
using HotelBooking.Business.Results;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IGuestRepository _guestRepository;
        private readonly IBaseRepository<RoomType> _roomTypeRepository;
        private readonly INotificationService _notificationService;

        public ReservationService(
            IReservationRepository reservationRepository,
            IRoomRepository roomRepository,
            IFloorRepository floorRepository,
            IGuestRepository guestRepository,
            IBaseRepository<RoomType> roomTypeRepository,
            INotificationService notificationService)
        {
            _reservationRepository = reservationRepository;
            _roomRepository = roomRepository;
            _floorRepository = floorRepository;
            _guestRepository = guestRepository;
            _roomTypeRepository = roomTypeRepository;
            _notificationService = notificationService;
        }

        private sealed record Stay(Room Room, DateTime CheckIn, DateTime CheckOut, decimal Total);

    
        public async Task<OperationResult<ReservationDto>> CreateAsync(CreateReservationDto dto)
        {
            if (dto is null)
                return OperationResult<ReservationDto>.Validation(DataMessages.EntityRequired);

            var stay = await ValidateStayAsync(dto.RoomId, dto.CheckIn, dto.CheckOut, dto.GuestCount, null);
            if (!stay.Success)
                return OperationResult<ReservationDto>.Fail(stay.Message, stay.ErrorType);

            var guest = await _guestRepository.GetEntityByIdAsync(dto.GuestId);
            if (!guest.Success)
                return OperationResult<ReservationDto>.Fail(guest.Message, guest.ErrorType);

            var s = stay.Data!;
            var reservation = new Reservation
            {
                RoomId = dto.RoomId,
                GuestId = dto.GuestId,
                CheckIn = s.CheckIn,
                CheckOut = s.CheckOut,
                GuestCount = dto.GuestCount,
                Total = s.Total,                       
                Status = ReservationStatus.Pending     
            };

            
            var saved = await _reservationRepository.SaveEntityAsync(reservation);
            if (!saved.Success)
                return OperationResult<ReservationDto>.Fail(saved.Message, saved.ErrorType);

            return OperationResult<ReservationDto>.Ok(
                ToDto(saved.Data!, s.Room.Number, guest.Data!.FullName), saved.Message);
        }

        public async Task<OperationResult<ReservationDto>> GetByIdAsync(int id)
        {
            return await LoadDtoAsync(id, string.Empty);
        }

        public async Task<OperationResult<List<ReservationDto>>> GetByGuestAsync(int guestId)
        {
            var guest = await _guestRepository.GetEntityByIdAsync(guestId);
            if (!guest.Success)
                return OperationResult<List<ReservationDto>>.Fail(guest.Message, guest.ErrorType);

            var reservations = await _reservationRepository.GetByGuestAsync(guestId);
            if (!reservations.Success)
                return OperationResult<List<ReservationDto>>.Fail(reservations.Message, reservations.ErrorType);

            
            var rooms = await _roomRepository.GetAllAsync();
            if (!rooms.Success)
                return OperationResult<List<ReservationDto>>.Fail(rooms.Message, rooms.ErrorType);

            var numberById = rooms.Data!.ToDictionary(r => r.Id, r => r.Number);
            var list = reservations.Data!
                .OrderByDescending(r => r.CheckIn)
                .Select(r => ToDto(r, numberById.GetValueOrDefault(r.RoomId, string.Empty), guest.Data!.FullName))
                .ToList();

            return OperationResult<List<ReservationDto>>.Ok(list);
        }

        
        public async Task<OperationResult<ReservationDto>> ModifyAsync(int id, ModifyReservationDto dto)
        {
            if (dto is null)
                return OperationResult<ReservationDto>.Validation(DataMessages.EntityRequired);

            var found = await _reservationRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<ReservationDto>.Fail(found.Message, found.ErrorType);

            var reservation = found.Data!;
            if (reservation.Status != ReservationStatus.Pending && reservation.Status != ReservationStatus.Confirmed)
                return OperationResult<ReservationDto>.Conflict(ErrorMessages.ReservationNotModifiable);

            
            var stay = await ValidateStayAsync(dto.RoomId, dto.CheckIn, dto.CheckOut, dto.GuestCount, id);
            if (!stay.Success)
                return OperationResult<ReservationDto>.Fail(stay.Message, stay.ErrorType);

            var s = stay.Data!;
            reservation.RoomId = dto.RoomId;
            reservation.CheckIn = s.CheckIn;
            reservation.CheckOut = s.CheckOut;
            reservation.GuestCount = dto.GuestCount;
            reservation.Total = s.Total;               

            var updated = await _reservationRepository.UpdateEntityAsync(reservation);
            if (!updated.Success)
                return OperationResult<ReservationDto>.Fail(updated.Message, updated.ErrorType);

            var warning = await NotifyAsync(id, NotificationType.Modification);
            return await LoadDtoAsync(id, "Reserva modificada correctamente." + warning);
        }

        
        public async Task<OperationResult<ReservationDto>> ConfirmAsync(int id)
        {
            var found = await _reservationRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<ReservationDto>.Fail(found.Message, found.ErrorType);

            var reservation = found.Data!;
            if (reservation.Status != ReservationStatus.Pending)
                return OperationResult<ReservationDto>.Conflict(ErrorMessages.ReservationNotConfirmable);

            reservation.Status = ReservationStatus.Confirmed;

            var updated = await _reservationRepository.UpdateEntityAsync(reservation);
            if (!updated.Success)
                return OperationResult<ReservationDto>.Fail(updated.Message, updated.ErrorType);

            var warning = await NotifyAsync(id, NotificationType.Confirmation);
            return await LoadDtoAsync(id, "Reserva confirmada correctamente." + warning);
        }

       
        public async Task<OperationResult<ReservationDto>> CancelAsync(int id)
        {
            var found = await _reservationRepository.GetEntityByIdAsync(id);
            if (!found.Success)
                return OperationResult<ReservationDto>.Fail(found.Message, found.ErrorType);

            var reservation = found.Data!;
            if (reservation.Status != ReservationStatus.Pending && reservation.Status != ReservationStatus.Confirmed)
                return OperationResult<ReservationDto>.Conflict(ErrorMessages.ReservationNotCancellable);

            reservation.Status = ReservationStatus.Cancelled;

            var updated = await _reservationRepository.UpdateEntityAsync(reservation);
            if (!updated.Success)
                return OperationResult<ReservationDto>.Fail(updated.Message, updated.ErrorType);

            var warning = await NotifyAsync(id, NotificationType.Cancellation);
            return await LoadDtoAsync(id, "Reserva cancelada correctamente." + warning);
        }

        
        private async Task<OperationResult<Stay>> ValidateStayAsync(
            int roomId, DateTime checkInRaw, DateTime checkOutRaw, int guestCount, int? excludeReservationId)
        {
            
            var checkIn = checkInRaw.Date;
            var checkOut = checkOutRaw.Date;

            if (checkOut <= checkIn)
                return OperationResult<Stay>.Validation(ErrorMessages.InvalidDates);
            if (checkIn < DateTime.Today)
                return OperationResult<Stay>.Validation(ErrorMessages.PastDates);
            if (guestCount <= 0)
                return OperationResult<Stay>.Validation(DataMessages.MustBePositive("Cantidad de huéspedes"));

            var room = await _roomRepository.GetEntityByIdAsync(roomId);
            if (!room.Success)
                return OperationResult<Stay>.Fail(room.Message, room.ErrorType);
            if (room.Data!.Status != RoomStatus.Available)
                return OperationResult<Stay>.Conflict(ErrorMessages.RoomStatusNotAvailable);

            var floor = await _floorRepository.GetEntityByIdAsync(room.Data.FloorId);
            if (!floor.Success)
                return OperationResult<Stay>.Fail(floor.Message, floor.ErrorType);
            if (!floor.Data!.IsActive)
                return OperationResult<Stay>.Conflict(ErrorMessages.FloorInactive);

            var type = await _roomTypeRepository.GetEntityByIdAsync(room.Data.RoomTypeId);
            if (!type.Success)
                return OperationResult<Stay>.Fail(type.Message, type.ErrorType);
            if (guestCount > type.Data!.Capacity)
                return OperationResult<Stay>.Validation(ErrorMessages.GuestCapacityExceeded);

            var overlapping = await _reservationRepository.GetOverlappingAsync(roomId, checkIn, checkOut, excludeReservationId);
            if (!overlapping.Success)
                return OperationResult<Stay>.Fail(overlapping.Message, overlapping.ErrorType);
            if (overlapping.Data!.Count > 0)
                return OperationResult<Stay>.Conflict(ErrorMessages.RoomNotAvailable);

            var nights = (checkOut - checkIn).Days;
            return OperationResult<Stay>.Ok(new Stay(room.Data, checkIn, checkOut, nights * type.Data.RatePerNight));
        }

       
        private async Task<string> NotifyAsync(int reservationId, NotificationType type)
        {
            var result = await _notificationService.NotifyAsync(reservationId, type);

            var delivered = result.Success && (result.Data is null || result.Data.Status == NotificationStatus.Sent);
            return delivered ? string.Empty : " " + ErrorMessages.NotificationNotSent;
        }

        private async Task<OperationResult<ReservationDto>> LoadDtoAsync(int id, string message)
        {
            var found = await _reservationRepository.GetWithDetailsAsync(id);
            if (!found.Success)
                return OperationResult<ReservationDto>.Fail(found.Message, found.ErrorType);

            var r = found.Data!;
            return OperationResult<ReservationDto>.Ok(ToDto(r, r.Room.Number, r.Guest.FullName), message);
        }

        private static ReservationDto ToDto(Reservation r, string roomNumber, string guestName) => new()
        {
            Id = r.Id,
            RoomId = r.RoomId,
            RoomNumber = roomNumber,
            GuestId = r.GuestId,
            GuestName = guestName,
            CheckIn = r.CheckIn,
            CheckOut = r.CheckOut,
            GuestCount = r.GuestCount,
            Total = r.Total,
            Status = r.Status
        };
    }
}
