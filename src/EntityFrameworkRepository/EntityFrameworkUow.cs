using System;
using System.Data.Entity.Validation;
using System.Text;
using Repository;

namespace EntityFrameworkRepository
{
    public class EntityFrameworkUow : UnitOfWorkBase
    {
        public EntityFrameworkContext Context { get; private set; }
        public EntityFrameworkUow(EntityFrameworkContext context)
        {
            Context = context;
        }
        public override void Commit()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (DbEntityValidationException exception)
            {
                var stringBuilder = new StringBuilder();
                foreach (var eve in exception.EntityValidationErrors)
                {
                    stringBuilder.Append(String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    stringBuilder.Append(Environment.NewLine);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        stringBuilder.Append(String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        stringBuilder.Append(Environment.NewLine);
                    }
                }

                throw new Exception(stringBuilder.ToString(), exception);
            }
        }

        public override IRepository<T> CreateRepository<T>()
        {
            return new EntityFrameworkRepo<T>(this);
        }
    }
}