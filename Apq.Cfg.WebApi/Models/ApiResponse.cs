namespace Apq.Cfg.WebApi.Models;

/// <summary>
/// 统一 API 响应格式
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public string? ErrorCode { get; set; }

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> Fail(string error, string? errorCode = null)
        => new() { Success = false, Error = error, ErrorCode = errorCode };
}

/// <summary>
/// 非泛型 API 响应
/// </summary>
public sealed class ApiResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? ErrorCode { get; set; }

    public static ApiResponse Ok() => new() { Success = true };
    public static ApiResponse Fail(string error, string? errorCode = null)
        => new() { Success = false, Error = error, ErrorCode = errorCode };
}
