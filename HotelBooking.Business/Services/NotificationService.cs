using HotelBooking.Business.DTOs.Notification;
using HotelBooking.Business.Interfaces;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Results;

namespace HotelBooking.Business.Services
{
    public class NotificationService : INotificationService
    {
        private const int MessageMaxLength = 255;

        private readonly INotificationRepository _notificationRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IEnumerable<INotificationChannel> _channels;

        public NotificationService(
            INotificationRepository notificationRepository,
            IReservationRepository reservationRepository,
            IEnumerable<INotificationChannel> channels)
        {
            _notificationRepository = notificationRepository;
            _reservationRepository = reservationRepository;
            _channels = channels;
        }

        public async Task<OperationResult<List<NotificationDto>>> NotifyAsync(int reservationId, NotificationType type)
        {
            var found = await _reservationRepository.GetWithDetailsAsync(reservationId);
            if (!found.Success)
                return OperationResult<List<NotificationDto>>.Fail(found.Message, found.ErrorType);

            var reservation = found.Data!;
            var (subject, summary) = Compose(type, reservation);
            var message = new NotificationMessage(type, subject, summary, reservation);
            var results = new List<NotificationDto>();

            foreach (var channel in _channels)
            {
                if (type != NotificationType.Modification)
                {
                    var alreadySent = await _notificationRepository.ExistsSentAsync(reservationId, type, channel.Name);
                    if (!alreadySent.Success)
                        return OperationResult<List<NotificationDto>>.Fail(alreadySent.Message, alreadySent.ErrorType);
                    if (alreadySent.Data)
                        continue;
                }

                var saved = await _notificationRepository.SaveEntityAsync(new Notification
                {
                    Type = type,
                    Channel = channel.Name,
                    Message = Truncate(summary, MessageMaxLength),
                    Status = NotificationStatus.Pending,
                    ReservationId = reservation.Id,
                    GuestId = reservation.GuestId
                });
                if (!saved.Success)
                    return OperationResult<List<NotificationDto>>.Fail(saved.Message, saved.ErrorType);

                var notification = saved.Data!;

                var sent = false;
                if (channel.CanDeliverTo(reservation.Guest))
                {
                    try { sent = await channel.SendAsync(reservation.Guest, message); }
                    catch (Exception) { sent = false; }
                }

                notification.Status = sent ? NotificationStatus.Sent : NotificationStatus.Failed;
                notification.SentAt = sent ? DateTime.UtcNow : null;

                var updated = await _notificationRepository.UpdateEntityAsync(notification);
                if (!updated.Success)
                    return OperationResult<List<NotificationDto>>.Fail(updated.Message, updated.ErrorType);

                results.Add(ToDto(updated.Data!));
            }

            return OperationResult<List<NotificationDto>>.Ok(results);
        }

        public async Task<OperationResult<List<NotificationDto>>> GetByReservationAsync(int reservationId)
        {
            var list = await _notificationRepository.GetByReservationAsync(reservationId);
            if (!list.Success)
                return OperationResult<List<NotificationDto>>.Fail(list.Message, list.ErrorType);

            return OperationResult<List<NotificationDto>>.Ok(list.Data!.Select(ToDto).ToList());
        }

        private static (string Subject, string Summary) Compose(NotificationType type, Reservation r)
        {
            var room = r.Room?.Number ?? r.RoomId.ToString();
            var dates = $"del {r.CheckIn:dd/MM/yyyy} al {r.CheckOut:dd/MM/yyyy}";

            return type switch
            {
                NotificationType.Confirmation => ("Reserva confirmada",
                    $"Su reserva #{r.Id} fue confirmada: habitación {room}, {dates}. Total: {r.Total:N2}."),
                NotificationType.Modification => ("Reserva modificada",
                    $"Su reserva #{r.Id} fue modificada: habitación {room}, {dates}. Total: {r.Total:N2}."),
                _ => ("Reserva cancelada",
                    $"Su reserva #{r.Id} fue cancelada (habitación {room}, {dates}).")
            };
        }

        private static string Truncate(string text, int max) =>
            text.Length <= max ? text : text.Substring(0, max);

        private static NotificationDto ToDto(Notification n) => new()
        {
            Id = n.Id,
            Type = n.Type,
            Channel = n.Channel,
            Message = n.Message,
            SentAt = n.SentAt,
            Status = n.Status,
            ReservationId = n.ReservationId
        };
    }
}
