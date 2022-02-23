using System.Linq.Expressions;

namespace QuickSpace.Data
{
    public interface IRepositoryBase<T>
    {
        T GetById(int id);
        IEnumerable<T> FindAll();
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);
        IEnumerable<T> FindByConditionAsce(Expression<Func<T, bool>> expression, string sortBy);
        IEnumerable<T> FindByConditionDesc(Expression<Func<T, bool>> expression, string sortBy);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Save();
    }
}
