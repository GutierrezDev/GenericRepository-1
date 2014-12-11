using System.Data.Entity;

namespace EntityFrameworkRepository
{
    public class EntityFrameworkContext : DbContext
    {
        public EntityFrameworkContext(string connectionStringName)
            : base(connectionStringName)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
    }
}