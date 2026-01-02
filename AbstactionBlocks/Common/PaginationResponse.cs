namespace AbstractionBlocks.CommonApplication.Pagination
{
    public class PaginationResponse<T> where T : class
    {
        public int? PageNumber { get; private set; }
        public int? PageSize { get; private set; }
        public int? TotalCount { get; private set; }
        public int? TotalPages { get; private set; }
        public List<T>? Data { get; private set; } = new();
        private PaginationResponse(int? pageNumber, int? pageSize, int? totalCount, int? totalPages
        , List<T>? data)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = totalPages;
            Data = data;
        }
        public static PaginationResponse<T> Create(int? pageNumber, int? pageSize, int? totalCount,
            int? totalPages, List<T>? data) => new(pageNumber, pageSize, totalCount, totalPages, data);
    }
}