namespace HotelBooking.Business.Results
{
    public static class ErrorMessages
    {

        public const string UnexpectedError = "Ocurrió un error inesperado. Intente nuevamente.";
        public const string HotelNotFound = "El hotel no existe.";
        public const string HotelNameRequired = "El nombre del hotel es obligatorio.";
        public const string HotelNameExists = "Ya existe un hotel con ese nombre.";
        public const string BlockNameExists = "Ya existe un bloque con ese nombre en el hotel.";
        public const string HotelInactive = "El hotel está inactivo y no admite cambios en su estructura.";
        public const string FloorNumberExists = "Ya existe un piso activo con el número {0} en este bloque.";
        public const string RoomNumberExists = "Ya existe la habitación {0} en este hotel.";
        public const string RoomNotAvailable = "La habitación ya no está disponible para esas fechas.";
        public const string InvalidDates = "La fecha de salida debe ser posterior a la de entrada.";
        public const string PastDates = "No se pueden registrar reservas para fechas pasadas.";
        public const string GuestCapacityExceeded = "La cantidad de huéspedes supera la capacidad de la habitación.";
        public const string ReservationNotCancellable = "Solo se pueden cancelar reservas pendientes o confirmadas.";

    }
}
