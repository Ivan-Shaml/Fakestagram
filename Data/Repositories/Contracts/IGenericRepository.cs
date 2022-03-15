using Fakestagram.Models;

namespace Fakestagram.Data.Repositories.Contracts
{
    public interface IPostsRepository<T> where T : BaseModel
    {
        IEnumerable<T> GetAll();
        T GetById(Guid id);
        void Create(T item);
        void Update(T item);
        void Save(T item);
        void Delete(Guid id);
    }
}
