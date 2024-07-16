namespace BlogApi.Dtos {
    public class PageList<T> {
        public List<T> Data { get;  }
        public int Page { get; }
        public int PageSize { get;}
        public int TotalCount { get;}
        public bool HasNextPage { get;}
        public bool HasPrevPage { get; }

        public PageList(List<T> data, int page, int pageSize, int totalCount) {
            Data = data;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            HasNextPage = page * pageSize < totalCount;
            HasPrevPage = page > 1;
        }
    }
}
