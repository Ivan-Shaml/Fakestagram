using Fakestagram.Models;

namespace Fakestagram.Data.Repositories.Contracts
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        IEnumerable<T> GetAll(int? skip, int? take);
        T GetById(Guid id);
        void Create(T item);
        void Update(T item);
        void Save(T item);
        void Delete(Guid id);
        T FirstOrDefault(Func<T, bool> predicate);
        int Count(Func<T, bool> predicate);
    }
}
