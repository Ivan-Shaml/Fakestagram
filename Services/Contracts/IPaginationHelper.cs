namespace Fakestagram.Services.Contracts
{
    public interface IPaginationHelper
    {
        void SetPaginationHeader(int totalCount, int currentPageNumber, int itemsPerPage);
    }
}
