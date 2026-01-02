namespace AbstractionBlocks.CommonApplication.Pagination
{
    public class PaginationValue
    {
        private int _pageNumber = 1;
        public int PageNumber { get => _pageNumber; set => _pageNumber = value <= 0 ? 1 : value; }
        private int _pageSize = 50;
        public int PageSize { get => _pageSize; set => _pageSize = value >= 1 && value <= 50 ? value : 50; }
    }
}