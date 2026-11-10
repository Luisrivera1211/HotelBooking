namespace HotelBooking.Data.Results
{
    public static class DataMessages
    {
        public const string InvalidId = "El identificador debe ser mayor que cero.";
        public const string EntityRequired = "La entidad es requerida.";
        public const string NotFound = "El registro no se encuentra registrado.";
        public const string QueryError = "Error consultando los datos.";
        public const string SaveError = "Error guardando los datos.";
        public const string UpdateError = "Error actualizando los datos.";
        public const string Saved = "Registro guardado correctamente.";
        public const string Updated = "Registro actualizado correctamente.";
        public const string ConstraintViolation =
            "No se pudo guardar. Verifique que los datos no estén duplicados y que los registros relacionados existan.";
        public const string InvalidDate = "La fecha de salida debe ser posterior a la de entrada.";

        public static string Required(string field) => $"El campo {field} es obligatorio.";
        public static string TooLong(string field, int max) => $"El campo {field} no puede superar {max} caracteres.";
        public static string MustBePositive(string field) => $"El campo {field} debe ser mayor que cero.";
    }
}
