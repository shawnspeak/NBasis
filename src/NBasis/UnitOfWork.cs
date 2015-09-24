using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBasis
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly Dictionary<WorkPosition, List<Action>> _work = new Dictionary<WorkPosition, List<Action>>();

        public void AddWork(Action action, WorkPosition position = WorkPosition.AfterTransaction)
        {
            if (!_work.ContainsKey(position))
                _work[position] = new List<Action>();
            else
            {
                if (position == WorkPosition.Transaction)
                    throw new ApplicationException("Transaction already added to the unit of work");
            }

            _work[position].Add(action);
        }

        public void Complete()
        {
            // execute each position
            if (_work.ContainsKey(WorkPosition.BeforeTransaction))
                _work[WorkPosition.BeforeTransaction].ForEach(w => w.Invoke());
            if (_work.ContainsKey(WorkPosition.Transaction))
                _work[WorkPosition.Transaction].ForEach(w => w.Invoke());
            if (_work.ContainsKey(WorkPosition.AfterTransaction))
                _work[WorkPosition.AfterTransaction].ForEach(w => w.Invoke());
        }

        public void Rollback()
        {
            // clear all the work
            _work.Clear();
        }
    }
}
