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
        private readonly IEmailSender _emailSender;

        public NotificationService(
            INotificationRepository notificationRepository,
            IReservationRepository reservationRepository,
            IEmailSender emailSender)
        {
            _notificationRepository = notificationRepository;
            _reservationRepository = reservationRepository;
            _emailSender = emailSender;
        }

        public async Task<OperationResult<NotificationDto>> NotifyAsync(int reservationId, NotificationType type)
        {
            var found = await _reservationRepository.GetWithDetailsAsync(reservationId);
            if (!found.Success)
                return OperationResult<NotificationDto>.Fail(found.Message, found.ErrorType);

            var reservation = found.Data!;
            var guest = reservation.Guest;

            if (type != NotificationType.Modification)
            {
                var alreadySent = await _notificationRepository.ExistsSentAsync(reservationId, type);
                if (!alreadySent.Success)
                    return OperationResult<NotificationDto>.Fail(alreadySent.Message, alreadySent.ErrorType);
                if (alreadySent.Data)
                    return OperationResult<NotificationDto>.Ok();
            }

            var (subject, summary) = Compose(type, reservation);

            
            var notification = new Notification
            {
                Type = type,
                Message = Truncate(summary, MessageMaxLength),
                Status = NotificationStatus.Pending,
                ReservationId = reservation.Id,
                GuestId = reservation.GuestId
            };

            var saved = await _notificationRepository.SaveEntityAsync(notification);
            if (!saved.Success)
                return OperationResult<NotificationDto>.Fail(saved.Message, saved.ErrorType);

            notification = saved.Data!;

            var sent = false;
            if (!string.IsNullOrWhiteSpace(guest.Email))
            {
                try
                {
                    var body = $"Hola {guest.FullName},\n\n{summary}\n\nGracias por elegirnos.";
                    sent = await _emailSender.SendAsync(guest.Email, subject, body);
                }
                catch (Exception)
                {
                    sent = false;
                }
            }

            notification.Status = sent ? NotificationStatus.Sent : NotificationStatus.Failed;
            notification.SentAt = sent ? DateTime.UtcNow : null;

            var updated = await _notificationRepository.UpdateEntityAsync(notification);
            if (!updated.Success)
                return OperationResult<NotificationDto>.Fail(updated.Message, updated.ErrorType);

            return OperationResult<NotificationDto>.Ok(ToDto(updated.Data!));
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
            Message = n.Message,
            SentAt = n.SentAt,
            Status = n.Status,
            ReservationId = n.ReservationId
        };
    }
}
