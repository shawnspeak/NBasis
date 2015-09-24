using System;
using System.Data.Entity;

namespace NBasis.Models
{
    public class ModelSessionFactory<TContext> : IModelSessionFactory where TContext : DbContext, new()
    {
        readonly IUnitOfWork _unitOfWork;
        public ModelSessionFactory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private ModelStatefullSession _StatefullSession;

        private TContext _StatelessSession;

        public IModelStatefullSession OpenSession()
        {
            if (_StatefullSession == null)
                _StatefullSession = new ModelStatefullSession(new TContext(), _unitOfWork);
            return _StatefullSession;
        }

        public IModelStatelessSession OpenStatelessSession()
        {
            if (_StatelessSession == null)
            {
                _StatelessSession = new TContext();
                _StatelessSession.Configuration.AutoDetectChangesEnabled = false;
            }
            return new ModelStatelessSession(_StatelessSession);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_StatefullSession != null)
                    _StatefullSession.InternalDispose();

                if (_StatelessSession != null)
                    _StatelessSession.Dispose();
            }
        }
    }
}
