namespace Fakestagram.Data.DTOs.Pagination
{
    public class PaginationParameters
    {
        private int _maxItemsPerPage = 30;
        private int itemsPerPage;
        public int Page { get; set; } = 1;
        public int ItemsPerPage
        {
            get => itemsPerPage;
            set => itemsPerPage = value > _maxItemsPerPage ? _maxItemsPerPage : value;
        }
    }
}
