namespace HotelBooking.Business.Results
{

    public enum ErrorType {None, Validation, NotFound, Conflict}
    public class OperationResult<T>
    {
        public bool Success { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public ErrorType ErrorType { get; private set; } = ErrorType.None;
        public T? Data { get; private set; }
    }
}
