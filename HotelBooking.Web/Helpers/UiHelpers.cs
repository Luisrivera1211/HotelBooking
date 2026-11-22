using HotelBooking.Data.Enums;

namespace HotelBooking.Web.Helpers
{
    public static class UiHelpers
    {
        public static string Text(this ReservationStatus s) => s switch
        {
            ReservationStatus.Pending => "Pendiente",
            ReservationStatus.Confirmed => "Confirmada",
            ReservationStatus.InProgress => "En curso",
            ReservationStatus.Completed => "Completada",
            ReservationStatus.Cancelled => "Cancelada",
            _ => s.ToString()
        };

        public static string Badge(this ReservationStatus s) => s switch
        {
            ReservationStatus.Pending => "warning text-dark",
            ReservationStatus.Confirmed => "success",
            ReservationStatus.InProgress => "primary",
            ReservationStatus.Completed => "secondary",
            ReservationStatus.Cancelled => "danger",
            _ => "light text-dark"
        };

        public static string Text(this RoomStatus s) => s switch
        {
            RoomStatus.Available => "Disponible",
            RoomStatus.Reserved => "Reservada",
            RoomStatus.Occupied => "Ocupada",
            RoomStatus.Cleaning => "En limpieza",
            RoomStatus.Maintenance => "Mantenimiento",
            RoomStatus.OutOfService => "Fuera de servicio",
            _ => s.ToString()
        };

        public static string Badge(this RoomStatus s) => s switch
        {
            RoomStatus.Available => "success",
            RoomStatus.Reserved => "warning text-dark",
            RoomStatus.Occupied => "primary",
            _ => "secondary"
        };

        public static string Text(this NotificationType t) => t switch
        {
            NotificationType.Confirmation => "Confirmación",
            NotificationType.Modification => "Modificación",
            NotificationType.Cancellation => "Cancelación",
            _ => t.ToString()
        };

        public static string Text(this NotificationStatus s) => s switch
        {
            NotificationStatus.Pending => "Pendiente",
            NotificationStatus.Sent => "Enviada",
            NotificationStatus.Failed => "Fallida",
            _ => s.ToString()
        };

        public static string AuditAction(string action) => action switch
        {
            "Create" => "Creó",
            "Update" => "Modificó",
            "Delete" => "Eliminó",
            _ => action
        };

        public static string AuditEntity(string entity) => entity switch
        {
            "Reservation" => "Reserva",
            "Room" => "Habitación",
            "Notification" => "Notificación",
            _ => entity
        };

        public static string Local(this DateTime utc) =>
            DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");

        public static string Local(this DateTime? utc) => utc.HasValue ? utc.Value.Local() : "-";

        public static string Stars(int stars)
        {
            var n = Math.Clamp(stars, 0, 5);
            return new string('★', n) + new string('☆', 5 - n);
        }

        public static string[] FeatureList(string? features) =>
            (features ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
