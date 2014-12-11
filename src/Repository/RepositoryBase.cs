using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IList<string> Includes;
        protected readonly IList<string> Orderby;
        protected readonly IList<string> Orderbydesc;
        protected int take;
        protected int skip;

        protected RepositoryBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            Includes = new List<string>();
            Orderby = new List<string>();
            Orderbydesc = new List<string>();
            take = 0;
            skip = 0;
        }
        public IEnumerable<T> Get()
        {
            IEnumerable<T> result = GetQuery();
            ClearQueryHistory();
            return result;
        }
        public T FindSingle(Expression<Func<T, bool>> predicate)
        {
            T result = FindSingleQuery(predicate);
            ClearQueryHistory();
            return result;
        }
        public TResult FindSingle<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            TResult result = FindSingleQuery(predicate, selector);
            ClearQueryHistory();
            return result;
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> results = FindQuery(predicate);
            ClearQueryHistory();
            return results;
        }

        public IEnumerable<TResult> Find<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            IEnumerable<TResult> results = FindQuery(predicate, selector);
            ClearQueryHistory();
            return results;
        }

        public int Count()
        {
            int result = CountQuery();
            ClearQueryHistory();
            return result;
        }
        public int Count(Expression<Func<T, bool>> predicate)
        {
            int result = CountQuery(predicate);
            ClearQueryHistory();
            return result;
        }
        public RepositoryBase<T> Include(params Expression<Func<T, object>>[] includes)
        {
            foreach (var include in includes)
            {
                Includes.Add(include.ToIncludeString());
            }

            return this;
        }

        public RepositoryBase<T> OrderBy(params Expression<Func<T, object>>[] orderbys)
        {
            foreach (var @orderby in orderbys)
            {
                Orderby.Add(@orderby.ToIncludeString());
            }

            return this;
        }

        public RepositoryBase<T> OrderByDesc(params Expression<Func<T, object>>[] orderbys)
        {
            foreach (var @orderby in orderbys)
            {
                Orderbydesc.Add(@orderby.ToIncludeString());
            }

            return this;
        }

        public RepositoryBase<T> Take(int count)
        {
            take = count;
            return this;
        }

        public RepositoryBase<T> Skip(int count)
        {
            skip = count;
            return this;
        }

        public void Create(T entity)
        {
            PersistCreate(entity);
        }
        public void Update(int id, T entity)
        {
            PersistUpdate(id, entity);
        }
        public void Delete(int id, T entity)
        {
            PersistDelete(id, entity);
        }
        protected abstract void PersistCreate(T entity);
        protected abstract void PersistUpdate(int id, T entity);
        protected abstract void PersistDelete(int id, T entity);
        protected abstract IEnumerable<T> GetQuery();
        protected abstract T FindSingleQuery(Expression<Func<T, bool>> predicate);
        protected abstract TResult FindSingleQuery<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);
        protected abstract int CountQuery();
        protected abstract int CountQuery(Expression<Func<T, bool>> predicate);
        protected abstract IEnumerable<T> FindQuery(Expression<Func<T, bool>> predicate);
        protected abstract IEnumerable<TResult> FindQuery<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);

        public void Dispose()
        {

        }
        private void ClearQueryHistory()
        {
            Includes.Clear();
            Orderby.Clear();
            Orderbydesc.Clear();
            take = 0;
            skip = 0;
        }
    }
}