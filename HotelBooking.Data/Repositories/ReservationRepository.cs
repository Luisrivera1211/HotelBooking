using System.Data;
using HotelBooking.Data.Context;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories
{
    public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
    {
        private const string OverlapMessage = "La habitación ya tiene una reserva activa en esas fechas.";

        public ReservationRepository(HotelBookingDbContext context) : base(context)
        { }

        protected override string? Validate(Reservation reservation)
        {
            if (reservation.CheckOut <= reservation.CheckIn)
                return DataMessages.InvalidDate;
            if (reservation.GuestCount <= 0)
                return DataMessages.MustBePositive("Cantidad de huéspedes");
            if (reservation.Total < 0)
                return "El total no puede ser negativo.";
            return null;
        }

        public override async Task<OperationResult<Reservation>> SaveEntityAsync(Reservation reservation)
        {
            try
            {
                if (reservation is null)
                    return OperationResult<Reservation>.Validation(DataMessages.EntityRequired);

                var error = Validate(reservation);
                if (error is not null)
                    return OperationResult<Reservation>.Validation(error);

                await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

                if (await HasOverlapAsync(reservation))
                    return OperationResult<Reservation>.Conflict(OverlapMessage);

                var result = await base.SaveEntityAsync(reservation);
                if (!result.Success)
                    return result;

                await transaction.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                return OperationResult<Reservation>.Unexpected(DataMessages.SaveError);
            }
        }

        public override async Task<OperationResult<Reservation>> UpdateEntityAsync(Reservation reservation)
        {
            try
            {
                if (reservation is null)
                    return OperationResult<Reservation>.Validation(DataMessages.EntityRequired);
                if (reservation.Id <= 0)
                    return OperationResult<Reservation>.Validation(DataMessages.InvalidId);

                var error = Validate(reservation);
                if (error is not null)
                    return OperationResult<Reservation>.Validation(error);

                await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

                if (await HasOverlapAsync(reservation))
                    return OperationResult<Reservation>.Conflict(OverlapMessage);

                var result = await base.UpdateEntityAsync(reservation);
                if (!result.Success)
                    return result;

                await transaction.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                return OperationResult<Reservation>.Unexpected(DataMessages.UpdateError);
            }
        }

        public async Task<OperationResult<List<Reservation>>> GetOverlappingAsync(
            int roomId, DateTime checkIn, DateTime checkOut, int? excludeReservationId = null)
        {
            try
            {
                if (roomId <= 0)
                    return OperationResult<List<Reservation>>.Validation(DataMessages.InvalidId);
                if (checkOut <= checkIn)
                    return OperationResult<List<Reservation>>.Validation(DataMessages.InvalidDate);

                var query = _context.Reservations
                    .AsNoTracking()
                    .Where(r => r.RoomId == roomId
                        && r.CheckIn < checkOut
                        && r.CheckOut > checkIn
                        && r.Status != ReservationStatus.Cancelled);

                if (excludeReservationId.HasValue)
                    query = query.Where(r => r.Id != excludeReservationId.Value);

                return OperationResult<List<Reservation>>.Ok(await query.ToListAsync());
            }
            catch (Exception)
            {
                return OperationResult<List<Reservation>>.Unexpected(DataMessages.QueryError);
            }
        }

        public async Task<OperationResult<List<Reservation>>> GetByGuestAsync(int guestId)
        {
            try
            {
                if (guestId <= 0)
                    return OperationResult<List<Reservation>>.Validation(DataMessages.InvalidId);

                var reservations = await _context.Reservations
                    .AsNoTracking()
                    .Where(r => r.GuestId == guestId)
                    .ToListAsync();
                return OperationResult<List<Reservation>>.Ok(reservations);
            }
            catch (Exception)
            {
                return OperationResult<List<Reservation>>.Unexpected(DataMessages.QueryError);
            }
        }

        public async Task<OperationResult<Reservation>> GetWithDetailsAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<Reservation>.Validation(DataMessages.InvalidId);

                var reservation = await _context.Reservations
                    .Include(r => r.Room).ThenInclude(room => room.RoomType)
                    .Include(r => r.Guest)
                    .FirstOrDefaultAsync(r => r.Id == id);

                return reservation is null
                    ? OperationResult<Reservation>.NotFound(DataMessages.NotFound)
                    : OperationResult<Reservation>.Ok(reservation);
            }
            catch (Exception)
            {
                return OperationResult<Reservation>.Unexpected(DataMessages.QueryError);
            }
        }
        private async Task<bool> HasOverlapAsync(Reservation reservation)
        {
            if (reservation.Status == ReservationStatus.Cancelled)
                return false;

            return await _context.Reservations.AnyAsync(r =>
                r.RoomId == reservation.RoomId
                && r.Id != reservation.Id
                && r.CheckIn < reservation.CheckOut
                && r.CheckOut > reservation.CheckIn
                && r.Status != ReservationStatus.Cancelled);
        }
    }
}