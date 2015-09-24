using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBasis.Commanding
{
    public static class CommandExtensions
    {
        public static Task Send(this ICommandBus bus, Envelope<ICommand> envelopedCommand)
        {
            return bus.Send(envelopedCommand.Yield());
        }

        public static Task Send(this ICommandBus bus, ICommand command, IDictionary<string, object> headers = null)
        {
            return bus.Send(new Envelope<ICommand>(command, headers));
        }

        public static Task<TResult> Ask<TResult>(this ICommandBus bus, ICommand command, IDictionary<string, object> headers = null)
        {
            return bus.Ask<TResult>(new Envelope<ICommand>(command, headers));
        }
    }
}
