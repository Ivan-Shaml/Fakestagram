using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

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

        public virtual IEnumerable<T> GetAll(int? skip, int? take)
        {
            skip ??= 0;
            take ??= 0;

            if (skip <= 0 && take <= 0)
            {
                return _dbSet.ToList();
            }
            return _dbSet
                .OrderBy(x => x.Id)
                .Skip(skip.Value)
                .Take(take.Value)
                .ToList();
        }

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

        public T FirstOrDefault(Func<T, bool> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public int Count(Func<T, bool> predicate)
        {
            if (predicate is null)
            {
                return _dbSet.Count();
            }

            return _dbSet.Count(predicate);
        }
    }
}
