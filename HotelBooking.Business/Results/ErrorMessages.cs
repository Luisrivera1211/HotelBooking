namespace HotelBooking.Business.Results
{
    public static class ErrorMessages
    {
        public const string UnexpectedError = "Ocurrió un error inesperado. Intente nuevamente.";

        public const string HotelNotFound = "El hotel no existe.";
        public const string HotelNameRequired = "El nombre del hotel es obligatorio.";
        public const string HotelNameExists = "Ya existe un hotel con ese nombre.";
        public const string HotelInactive = "El hotel está inactivo.";
        public const string BlockNameExists = "Ya existe un bloque con ese nombre en el hotel.";
        public const string BlockInactive = "El bloque está inactivo y no admite pisos nuevos.";
        public const string FloorNumberExists = "Ya existe un piso activo con el número {0} en este bloque.";
        public const string FloorInactive = "El piso está inactivo y no admite habitaciones ni reservas.";

        public const string RoomNumberExists = "Ya existe la habitación {0} en este hotel.";
        public const string RoomTypeNameExists = "Ya existe un tipo de habitación con ese nombre.";
        public const string InvalidRoomStatus = "El estado de la habitación no es válido.";

        public const string GuestDocumentExists = "Ya existe un huésped con ese documento.";
        public const string GuestNotFoundByDocument = "No existe un huésped con ese documento.";
        public const string InvalidEmail = "El correo electrónico no tiene un formato válido.";

        public const string EmployeeEmailExists = "Ya existe un empleado con ese correo.";
        public const string PasswordTooShort = "La contraseña debe tener al menos 8 caracteres.";
        public const string PasswordMismatch = "Las contraseñas no coinciden.";
        public const string InvalidRole = "El rol indicado no es válido.";
        public const string InvalidAdminCode = "El código de autorización para administrador no es válido.";
        public const string InvalidCredentials = "Correo o contraseña incorrectos.";

        public const string NoRoomsAvailable = "No hay habitaciones disponibles para los criterios indicados.";
        public const string RoomStatusNotAvailable = "La habitación no se encuentra en estado disponible.";
        public const string RoomNotAvailable = "La habitación ya no está disponible para esas fechas.";
        public const string InvalidDates = "La fecha de salida debe ser posterior a la de entrada.";
        public const string PastDates = "No se pueden registrar reservas para fechas pasadas.";
        public const string GuestCapacityExceeded = "La cantidad de huéspedes supera la capacidad de la habitación.";
        public const string ReservationNotConfirmable = "Solo se pueden confirmar reservas pendientes.";
        public const string ReservationNotModifiable = "Solo se pueden modificar reservas pendientes o confirmadas.";
        public const string ReservationNotCancellable = "Solo se pueden cancelar reservas pendientes o confirmadas.";

        public const string NotificationNotSent = "La notificación no pudo enviarse.";
    }
}
