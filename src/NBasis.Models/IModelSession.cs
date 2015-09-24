using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace NBasis.Models
{
    public interface IModelSession : IDisposable
    {
        T Load<T>(Guid id) where T : class, IEntity;
        Task<T> LoadAsync<T>(Guid id) where T : class, IEntity;
        void Store<T>(T item) where T : class, IEntity;
        void CreateOrAlter<T>(Guid id, Func<T, T> createOrAlter) where T : class, IEntity;
        void Alter<T>(Guid id, Action<T> alterItem) where T : class, IEntity;
        IQueryable<T> Query<T>() where T : class, IEntity;
        //IQueryable<T> Query<T>(IQuery<T> query) where T : class, IEntity;
        //T Find<T>(IQuery<T> query) where T : class, IEntity;
        bool Remove<T>(Guid id) where T : class, IEntity;
        void Remove<T>(T item) where T : class, IEntity;
    }

    public enum SessionState
    {
        Open,
        RolledBack,
        Committed
    }

    public interface IModelStatefullSession : IModelSession
    {
        DbContext Context { get; }
    }

    public interface IModelStatelessSession : IModelSession
    {
        DbContext Context { get; }
    }
}
