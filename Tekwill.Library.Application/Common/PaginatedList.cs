using System.Text;

namespace Tekwill.Library.Application.Common
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginatedList(List<T> items, int pageIndex, int totalPages)
        {
            Items = items;
            PageIndex = pageIndex;
            TotalPages = totalPages;
        }

        public override string ToString()
        {
            var text = new StringBuilder($"PageNumber: {PageIndex} \n" +
                $"HasPrevious: {HasPreviousPage} \n" +
                $"HasNext: {HasNextPage}\n" +
                $"Total: {TotalPages}\n" +
                $"--------------------\n");
            foreach(var item in Items)
            {
                text.Append(item?.ToString());
                text.Append("\n");
                text.Append("--------------------\n");
            }
            return text.ToString();
        }
    }
}
