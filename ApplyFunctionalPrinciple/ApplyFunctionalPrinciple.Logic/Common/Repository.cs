using ApplyFunctionalPrinciple.Logic.Utils;

namespace ApplyFunctionalPrinciple.Logic.Common
{
    public class Repository<TEntity> where TEntity : Entity
    {
        protected readonly UnitOfWork UnitOfWork;

        protected Repository(UnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public TEntity GetById(long id)
        {
            return UnitOfWork.Get<TEntity>(id);
        }

        public void Save(TEntity entity)
        {
            UnitOfWork.SaveOrUpdate(entity);
        }
    }
}