using HotelBooking.Data.Enums;

namespace HotelBooking.Data.Results
{
    public sealed class OperationResult<T>
    {
        public OperationResult()
        {
            Success = true;
        }

        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ErrorType ErrorType { get; set; } = ErrorType.None;
        public T? Data { get; set; }

        public static OperationResult<T> Ok(T? data = default, string message = "")
            => new() { Data = data, Message = message };

        public static OperationResult<T> Fail(string message, ErrorType errorType)
            => new() { Success = false, Message = message, ErrorType = errorType };

        public static OperationResult<T> NotFound(string message) => Fail(message, ErrorType.NotFound);
        public static OperationResult<T> Validation(string message) => Fail(message, ErrorType.Validation);
        public static OperationResult<T> Conflict(string message) => Fail(message, ErrorType.Conflict);
        public static OperationResult<T> Unexpected(string message) => Fail(message, ErrorType.Unexpected);
    }
}
