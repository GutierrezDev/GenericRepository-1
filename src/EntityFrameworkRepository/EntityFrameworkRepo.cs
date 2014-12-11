using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;

using Repository;

namespace EntityFrameworkRepository
{
    public class EntityFrameworkRepo<T> : RepositoryBase<T> where T : class
    {
        private readonly DbContext _context;
        public EntityFrameworkRepo(EntityFrameworkUow unitOfWork)
            : base(unitOfWork)
        {
            _context = unitOfWork.Context;
        }

        private IQueryable<T> Query()
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            if (Includes.Count > 0)
            {
                query = Includes.Aggregate(query, (current, path) => current.Include(path));
            }

            if (Orderby.Count > 0)
            {
                query = query.OrderBy(string.Join(",", Orderby));
            }

            if (Orderbydesc.Count > 0)
            {
                for (int i = 0; i < Orderbydesc.Count; i++)
                {
                    Orderbydesc[i] = Orderbydesc[i] + " Desc";
                }

                query = query.OrderBy(string.Join(",", Orderbydesc));
            }

            if (take > 0)
            {
                query = query.Take(take);
            }

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            return query;
        }

        protected override void PersistCreate(T entity)
        {
            _context.Set<T>().Add(entity);
        }
        protected override void PersistUpdate(int id, T entity)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var set = _context.Set<T>();
                T attachedEntity = set.Find(id);
                if (attachedEntity != null)
                {
                    var attachedEntry = _context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    entry.State = EntityState.Modified;
                }
            }
        }
        protected override void PersistDelete(int id, T entity)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var set = _context.Set<T>();
                T attachedEntity = set.Find(id);
                if (attachedEntity != null)
                {
                    var attachedEntry = _context.Entry(attachedEntity);
                    attachedEntry.State = EntityState.Deleted;

                }
                else
                {
                    entry.State = EntityState.Deleted;
                }
            }
        }
        protected override IEnumerable<T> GetQuery()
        {
            return Query().ToArray();
        }
        protected override T FindSingleQuery(Expression<Func<T, bool>> predicate)
        {
            return Query().SingleOrDefault(predicate);
        }
        protected override TResult FindSingleQuery<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            T entity = Query().SingleOrDefault(predicate);
            return entity == null ? default(TResult) : new[] { entity }.AsQueryable().Select(selector).First();
        }
        protected override int CountQuery()
        {
            return _context.Set<T>().Count();
        }
        protected override int CountQuery(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Count(predicate);
        }
        protected override IEnumerable<T> FindQuery(Expression<Func<T, bool>> predicate)
        {
            return Query().Where(predicate).ToArray();
        }

        protected override IEnumerable<TResult> FindQuery<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            return Query().Where(predicate).Select(selector).ToArray();
        }
    }
}
