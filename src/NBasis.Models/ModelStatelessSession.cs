using Common.Logging;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace NBasis.Models
{
    public class ModelStatelessSession : IModelStatelessSession
    {
        readonly Guid _SessionId;
        readonly DbContext _Context;
        readonly ILog _log;

        public ModelStatelessSession(DbContext context)
        {
            _log = LogManager.GetLogger("ModelStatelessSession");

            _SessionId = Guid.NewGuid();
            _Context = context;
            
            _log.DebugFormat("Session created: {0}", _SessionId);
            
            // enable context logging
            if (_log.IsDebugEnabled)
            {
                _Context.Database.Log = (s) =>
                {
                    _log.Debug(s);
                };
            }
        }

        public T Load<T>(Guid id) where T : class, IEntity
        {
            return _Context.Set<T>().SingleOrDefault(e => e.Id == id);
        }

        public Task<T> LoadAsync<T>(Guid id) where T : class, IEntity
        {
            return _Context.Set<T>().FindAsync(id);
        }

        public void Store<T>(T item) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public void CreateOrAlter<T>(Guid id, Func<T, T> createOrAlter) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public void Alter<T>(Guid id, Action<T> alterAction) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Query<T>() where T : class, IEntity
        {
            return _Context.Set<T>();
        }

        //public IQueryable<T> Query<T>(IQuery<T> query) where T : class, IEntity
        //{
        //    if (query == null) return Enumerable.Empty<T>().AsQueryable();
        //    return query.Execute(_Context.Set<T>());
        //}

        //public T Find<T>(IQuery<T> query) where T : class, IEntity
        //{
        //    if (query == null) return default(T);
        //    var fq = query as IFindQuery;
        //    if (fq == null)
        //        throw new ArgumentException("Query is not marked as a Find query");
        //    var sfq = query as ISingleFind;
        //    if (sfq != null)
        //        return Query(query).SingleOrDefault();
        //    return Query(query).FirstOrDefault();
        //}

        public bool Remove<T>(Guid id) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public void Remove<T>(T item) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // nothing to cleanup.. but we'll log it anyways
            _log.DebugFormat("Session disposed: {0}", _SessionId);
        }

        public DbContext Context
        {
            get
            {
                return _Context;
            }
        }
    }
}
