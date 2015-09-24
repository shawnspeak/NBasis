using Common.Logging;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace NBasis.Models
{
    public class ModelStatefullSession : IModelStatefullSession
    {
        readonly Guid _SessionId;
        readonly DbContext _Context;
        readonly DbContextTransaction _Transaction;
        readonly ILog _log;

        private SessionState _state = SessionState.Open;

        public ModelStatefullSession(DbContext context, IUnitOfWork unitOfWork)
        {
            _log = LogManager.GetLogger("ModelStatefullSession");

            _SessionId = Guid.NewGuid();
            _Context = context;
            _log.DebugFormat("Session created: {0}", _SessionId);

            // enable context logging
            if (_log.IsDebugEnabled)
            {
                //_Context.Database.Log = (s) =>
                //{
                //    _log.Debug(s);
                //};
            }

            _Transaction = _Context.Database.BeginTransaction();

            unitOfWork.AddWork(() =>
            {
                try
                {
                    _Context.SaveChanges();
                    _Transaction.Commit();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    _state = SessionState.Committed;
                }
                
            }, WorkPosition.Transaction);
        }

        public T Load<T>(Guid id) where T : class, IEntity
        {
            CheckTransaction();
            return _Context.Set<T>().SingleOrDefault(e => e.Id == id);
        }

        public Task<T> LoadAsync<T>(Guid id) where T : class, IEntity
        {
            CheckTransaction();
            return _Context.Set<T>().FindAsync(id);
        }

        public void Store<T>(T item) where T : class, IEntity
        {
            CheckTransaction();
            _Context.Set<T>().Add(item);
        }

        public void CreateOrAlter<T>(Guid id, Func<T, T> createOrAlter) where T : class, IEntity
        {
            if (createOrAlter == null)
                throw new ArgumentNullException("createOrAlter", "Must have an Create or alter item action to create items");

            T item = Load<T>(id);
            if (item == null)
            {
                item = createOrAlter(null as T);
                Store(item);
            }
            else
            {
                createOrAlter(item);
            }
        }

        public void Alter<T>(Guid id, Action<T> alterAction) where T : class, IEntity
        {
            if (alterAction == null)
                throw new ArgumentNullException("alterAction", "Must have an Alter item action to alter items");

            T item = Load<T>(id);
            if (item != null)
            {
                alterAction(item);
            }
        }

        public IQueryable<T> Query<T>() where T : class, IEntity
        {
            CheckTransaction();
            return _Context.Set<T>();
        }

        //public IQueryable<T> Query<T>(IQuery<T> query) where T : class, IEntity
        //{
        //    if (query == null) return Enumerable.Empty<T>().AsQueryable();
        //    CheckTransaction();
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
            T item = Load<T>(id);
            if (item != null)
            {
                _Context.Set<T>().Remove(item);
                return true;
            }

            return false;
        }

        public void Remove<T>(T item) where T : class, IEntity
        {
            if (item == null) throw new ArgumentNullException("Must have a model entity to remove");
            CheckTransaction();

            _Context.Set<T>().Remove(item);
        }

        private void CheckTransaction()
        {
            switch (_state)
            {
                case SessionState.Committed: throw new Exception("Statefull session was already committed");
                case SessionState.RolledBack: throw new Exception("Statefull session was already rolled back");
                default:
                    break;
            }
        }

        //public void Rollback()
        //{
        //    CheckTransaction();

        //    try
        //    {
        //        _Transaction.Rollback();
        //    }
        //    finally
        //    {
        //        _state = SessionState.RolledBack;
        //    }
        //}

        public void Commit()
        {
            CheckTransaction();

            try
            {
                _Transaction.Commit();
            }
            finally
            {
                _state = SessionState.Committed;
            }
        }

        public void Dispose()
        {
            //Dispose(true);
            //GC.SuppressFinalize(this);
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    //if (disposing)
        //    //{
        //    //    _Transaction.Dispose();
        //    //}
        //    _log.DebugFormat("Session disposed: {0}", _SessionId);
        //}

        internal virtual void InternalDispose()
        {
            _Transaction.Dispose();
            _log.DebugFormat("Session disposed: {0}", _SessionId);
        }

        public DbContext Context
        {
            get
            {
                return _Context;
            }
        }

        public SessionState State
        {
            get { return _state; }
        }
    }
}
