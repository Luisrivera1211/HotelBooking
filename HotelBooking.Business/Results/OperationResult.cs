using HotelBooking.Business.Enums;

namespace HotelBooking.Business.Results
{
    public class OperationResult<T>
    {
        public bool Success { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public ErrorTypeEnums ErrorType { get; private set; } = ErrorTypeEnums.None;
        public T? Data { get; private set; }


        private OperationResult() { }


        public static OperationResult<T> Ok(T? data = default, string message = "")
             => new() { Success = true, Data = data, Message = message };

        public static OperationResult<T> Fail(string message, ErrorTypeEnums errortype)
            => new() { Success = false, Message = message, ErrorType = errortype };

    }
       
}


