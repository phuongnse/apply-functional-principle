using System;
using System.Data;
using System.Linq;
using ApplyFunctionalPrinciple.Logic.Common;
using NHibernate;

namespace ApplyFunctionalPrinciple.Logic.Utils
{
    public class UnitOfWork : IDisposable
    {
        private readonly ISession _session;
        private readonly ITransaction _transaction;
        private bool _isAlive = true;
        private bool _isCommitted;

        public UnitOfWork()
        {
            _session = SessionFactory.OpenSession();
            _transaction = _session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void Dispose()
        {
            if (!_isAlive)
                return;

            _isAlive = false;

            try
            {
                if (_isCommitted)
                    _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
                _session.Dispose();
            }
        }

        public void Commit()
        {
            if (!_isAlive)
                return;

            _isCommitted = true;
        }

        internal Maybe<TEntity> Get<TEntity>(long id) where TEntity : Entity
        {
            return _session.Get<TEntity>(id);
        }

        internal void SaveOrUpdate<TEntity>(TEntity entity) where TEntity : Entity
        {
            _session.SaveOrUpdate(entity);
        }

        internal void Delete<TEntity>(TEntity entity) where TEntity : Entity
        {
            _session.Delete(entity);
        }

        internal IQueryable<TEntity> Query<TEntity>() where TEntity : Entity
        {
            return _session.Query<TEntity>();
        }
    }
}