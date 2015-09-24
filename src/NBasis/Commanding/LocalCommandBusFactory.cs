using NBasis.Types;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NBasis.Commanding
{
    public class LocalCommandBusFactory : ICommandBusFactory
    {
        private IDictionary<Type, CommandHandlerInvoker> commandInvokers;
        private static object _lock = new object();

        public LocalCommandBusFactory(ITypeFinder typeFinder)
        {
            BuildCommandInvokers(typeFinder);
        }

        public ICommandBus GetCommandBus(IUnityContainer container)
        {
            return new CommandBus(container, this);
        }

        private class CommandBus : ICommandBus
        {
            readonly IUnityContainer _container;
            readonly LocalCommandBusFactory _Factory;

            public CommandBus(IUnityContainer container, LocalCommandBusFactory factory)
            {
                _container = container;
                _Factory = factory;
            }

            public Task Send(IEnumerable<Envelope<ICommand>> commands)
            {
                List<Task> tasks = new List<Task>();
                commands.ForEach((command) =>
                {
                    var commandHandler = _Factory.GetTheCommandHandler(command.Body);

                    if (commandHandler == null) return;

                    tasks.Add(commandHandler.Send(_container, command.Body, command.Headers, command.CorrelationId));
                });
                return Task.WhenAll(tasks);
            }

            public Task<TResult> Ask<TResult>(Envelope<ICommand> command)
            {
                var commandHandler = _Factory.GetTheCommandHandler(command.Body);
                if (commandHandler == null) return Task.FromResult<TResult>(default(TResult));
                return commandHandler.Ask<TResult>(_container, command.Body, command.Headers, command.CorrelationId);
            }
        }

        private void BuildCommandInvokers(ITypeFinder typeFinder)
        {
            if (commandInvokers == null)
            {
                lock (_lock)
                {
                    if (commandInvokers == null)
                    {
                        commandInvokers = new Dictionary<Type, CommandHandlerInvoker>();
                        foreach (var commandHandlerType in typeFinder.GetInterfaceImplementations<IHandleCommands>())
                        {
                            foreach (var commandType in GetCommandTypesForCommandHandler(commandHandlerType))
                            {
                                if (commandInvokers.ContainsKey(commandType))
                                    throw new DuplicateCommandHandlersException(commandType);

                                commandInvokers.Add(commandType, new CommandHandlerInvoker(commandType, commandHandlerType));
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<Type> GetCommandTypesForCommandHandler(Type commandHandlerType)
        {
            return (from interfaceType in commandHandlerType.GetInterfaces()
                    where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleCommands<>)
                    select interfaceType.GetGenericArguments()[0]).ToArray();
        }

        private CommandHandlerInvoker GetTheCommandHandler(ICommand command)
        {
            CommandHandlerInvoker commandInvoker;
            if (!commandInvokers.TryGetValue(command.GetType(), out commandInvoker))
                throw new CommandHandlerNotFoundException(command.GetType());
            return commandInvoker;
        }

        public class CommandHandlerInvoker
        {
            readonly Type commandHandlerType;
            readonly Type commandType;

            public CommandHandlerInvoker(Type commandType, Type commandHandlerType)
            {
                this.commandType = commandType;
                this.commandHandlerType = commandHandlerType;
            }

            public Task Send(IUnityContainer container, ICommand command, IDictionary<String, object> headers, String correlationId)
            {
                var handlingContext = CreateTheCommandHandlingContext(command, headers, correlationId);
                return ExecuteTheCommandHandler(container, handlingContext);
            }

            public async Task<TResult> Ask<TResult>(IUnityContainer container, ICommand command, IDictionary<String, object> headers, String correlationId)
            {
                var handlingContext = CreateTheCommandHandlingContext(command, headers, correlationId);
                await ExecuteTheCommandHandler(container, handlingContext);
                return handlingContext.GetReturn<TResult>();
            }

            private async Task ExecuteTheCommandHandler(IUnityContainer container, ICommandHandlingContext<ICommand> handlingContext)
            {
                var handleMethod = GetTheHandleMethod();
                var commandHandler = container.Resolve(commandHandlerType);

                try
                {
                    await (Task)handleMethod.Invoke(commandHandler, new object[] { handlingContext });
                    return;
                }
                catch (TargetInvocationException ex)
                {
                    throw new CommandHandlerInvocationException(
                        string.Format("Command handler '{0}' for '{1}' failed. Inspect inner exception.", commandHandler.GetType().Name, handlingContext.Command.GetType().Name),
                        ex.InnerException)
                        {
                            HandlingContext = handlingContext
                        };
                }
            }

            private ICommandHandlingContext<ICommand> CreateTheCommandHandlingContext(ICommand command, IDictionary<String, object> headers, String correlationId)
            {
                var handlingContextType = typeof(CommandHandlingContext<>).MakeGenericType(commandType);
                return (ICommandHandlingContext<ICommand>)Activator.CreateInstance(handlingContextType, command, headers, correlationId);
            }

            private MethodInfo GetTheHandleMethod()
            {
                return typeof(IHandleCommands<>).MakeGenericType(commandType).GetMethod("Handle");
            }
        }

        private class CommandHandlingContext<TCommand> : ICommandHandlingContext<TCommand>
            where TCommand : ICommand
        {
            public CommandHandlingContext(TCommand command, IDictionary<String, object> headers, String correlationId)
            {
                Command = command;
                Headers = headers;
                CorrelationId = correlationId;
            }

            public IDictionary<String, Object> Headers { get; private set; }

            public TCommand Command { get; private set; }

            public String CorrelationId { get; private set; }
            
            private object _returnValue;
            public T GetReturn<T>()
            {
                return (T)_returnValue;
            }

            public void SetReturn(object value)
            {
                _returnValue = value;
            }
        }
    }
}
