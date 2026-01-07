namespace AbstractionBlocks.Common.Exception
{
    public class ApiResponse<TData>
    {
        public bool IsSuccess { get; init; }
        public TData? Data { get; init; }
        public List<string>? Errors { get; init; }
        public string? CorrelationId { get; init; } 
        public int StatusCode { get; init; }
        private ApiResponse(TData? data, bool success, List<string>? errors, int statusCode, string correlationId)
        {
            Data = data;
            IsSuccess = success;
            Errors = errors;
            StatusCode = statusCode;
            CorrelationId = correlationId;
        }
        public static ApiResponse<TData> Success(TData? data, string correlationId, int statusCode)
        => new(data, true, null, statusCode, correlationId);
        public static ApiResponse<TData> Error(List<string>? errors, int statusCode, string correlationId) 
        => new(default, false, errors, statusCode, correlationId);
    }
}
