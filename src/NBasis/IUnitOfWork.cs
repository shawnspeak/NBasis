using System;

namespace NBasis
{
    public enum WorkPosition
    {
        Transaction, // can only be set once
        BeforeTransaction,
        AfterTransaction
    }

    public interface IUnitOfWork
    {
        void AddWork(Action action, WorkPosition position = WorkPosition.AfterTransaction);

        void Complete();

        void Rollback();
    }
}
