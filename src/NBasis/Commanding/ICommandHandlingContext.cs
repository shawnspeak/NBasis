using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBasis.Commanding
{
    public interface ICommandHandlingContext<out TCommand> where TCommand : ICommand
    {
        TCommand Command { get; }
        IDictionary<String, Object> Headers { get; }
        String CorrelationId { get; }
        void SetReturn(object value);
        T GetReturn<T>();
    }
}
