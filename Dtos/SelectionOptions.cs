using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlogApi.Dtos {
    public class  SelectionOptions {
        public string? FilterColumn { get; set; }
        public string? FilterValue { get; set; }
        public string OrderBy { get; set; } = "id";
        public string Order { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;

        public bool CanFilter(string columnName) {
            if( FilterColumn is not null &&  FilterColumn.Equals(columnName, StringComparison.CurrentCultureIgnoreCase) && FilterValue is not null) {
                return true;
            }
            return false;
        }
    }
}
