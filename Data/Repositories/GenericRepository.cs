using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Microsoft.EntityFrameworkCore;

namespace Fakestagram.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        protected readonly FakestagramDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(FakestagramDbContext dbContext)
        {
            _context = dbContext;
            _dbSet = _context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll() => _dbSet.ToList();

        public virtual T GetById(Guid id)
        {
            return _dbSet.FirstOrDefault(u => u.Id == id);
        }

        public virtual void Create(T item)
        {
            _dbSet.Add(item);

            _context.SaveChanges();
        }

        public virtual void Update(T item)
        {
            T entity = GetById(item.Id);

            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }

            _context.Entry(item).State = EntityState.Modified;

            _context.SaveChanges();
        }

        public virtual void Save(T item)
        {
            if (item.Id != Guid.Empty)
            {
                Update(item);
            }
            else
            {
                Create(item);
            }
        }

        public virtual void Delete(Guid id)
        {
            var entity = GetById(id);

            if (entity is null)
            {
                throw new InvalidDataException("The spectified Id is not found");
            }

            _dbSet.Remove(entity);

            _context.SaveChanges();
        }
    }
}
