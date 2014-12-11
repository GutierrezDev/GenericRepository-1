using System.Collections;

namespace Repository
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        public Hashtable Repositories;
        public IRepository<T> Repository<T>() where T : class
        {
            if (Repositories == null)
            {
                Repositories = new Hashtable();
            }

            var key = typeof(T).Name;

            if (!Repositories.ContainsKey(key))
            {
                var repository = CreateRepository<T>();
                Repositories.Add(key, repository);
            }

            return (IRepository<T>)Repositories[key];
        }

        public abstract void Commit();
        public abstract IRepository<T> CreateRepository<T>() where T : class;
        public void Dispose()
        {

        }
    }
}