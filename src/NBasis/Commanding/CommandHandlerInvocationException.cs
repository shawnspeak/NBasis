using System;
using System.Runtime.Serialization;

namespace NBasis.Commanding
{
    [Serializable]
    public class CommandHandlerInvocationException : Exception
    {
        public ICommandHandlingContext<ICommand> HandlingContext { get; set; }

        public CommandHandlerInvocationException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CommandHandlerInvocationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
