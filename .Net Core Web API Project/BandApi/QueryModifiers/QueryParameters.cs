namespace BandApi.QueryModifiers
{
    public class QueryParameters
    {
        public string MainGenre { get; set; }
        public string BandName { get; set; }

        private const int MaxPageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value <= MaxPageSize ? value : MaxPageSize;
        }
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 4;

    }
}
