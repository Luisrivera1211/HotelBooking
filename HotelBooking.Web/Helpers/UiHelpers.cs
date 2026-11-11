using HotelBooking.Data.Enums;

namespace HotelBooking.Web.Helpers
{
    // Traducciones y colores para mostrar enums en pantalla (solo presentación)
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
    }
}
