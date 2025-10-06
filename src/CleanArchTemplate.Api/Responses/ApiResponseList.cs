using System.Diagnostics.CodeAnalysis;

namespace CleanArchTemplate.Api.Responses;

[ExcludeFromCodeCoverage]
public class ApiResponseList<T> : ApiResponse<IEnumerable<T>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    public ApiResponseList(
        IEnumerable<T> data,
        int pageNumber,
        int pageSize,
        int totalCount,
        string? message = null
    ) : base(data, message)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
