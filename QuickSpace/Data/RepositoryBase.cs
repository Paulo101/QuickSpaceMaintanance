using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace QuickSpace.Data
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected ApplicationDbContext _appDbContext;
        public RepositoryBase(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Create(T entity)
        {
            _appDbContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _appDbContext.Set<T>().Remove(entity);
        }

        public IEnumerable<T> FindAll()
        {
            return _appDbContext.Set<T>();
        }

        public IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return _appDbContext.Set<T>().Where(expression);
        }

        public IEnumerable<T> FindByConditionAsce(Expression<Func<T, bool>> expression, string sortBy)
        {
            return _appDbContext.Set<T>().Where(expression)
                .OrderBy(x => EF.Property<object>(x, sortBy));
        }

        public IEnumerable<T> FindByConditionDesc(Expression<Func<T, bool>> expression, string sortBy)
        {
            return _appDbContext.Set<T>().Where(expression)
                .OrderByDescending(x => EF.Property<object>(x, sortBy));
        }

        public T GetById(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return _appDbContext.Set<T>().Find(id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public void Save()
        {
            _appDbContext.SaveChanges();
        }

        public void Update(T entity)
        {
            _appDbContext.Set<T>().Update(entity);
        }
    }
}
