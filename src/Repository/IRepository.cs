using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Repository
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> Get();
        T FindSingle(Expression<Func<T, bool>> predicate);
        TResult FindSingle<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        IEnumerable<TResult> Find<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);
        int Count();
        int Count(Expression<Func<T, bool>> predicate);
        RepositoryBase<T> Include(params Expression<Func<T, object>>[] includes);
        RepositoryBase<T> OrderBy(params Expression<Func<T, object>>[] orderbys);
        RepositoryBase<T> OrderByDesc(params Expression<Func<T, object>>[] orderbys);
        RepositoryBase<T> Take(int count);
        RepositoryBase<T> Skip(int count);
        void Create(T entity);
        void Update(int id, T entity);
        void Delete(int id, T entity);
    }
}
