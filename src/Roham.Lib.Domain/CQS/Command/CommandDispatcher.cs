using Roham.Lib.Ioc;

namespace Roham.Lib.Domain.CQS.Command
{
    public interface ICommandDispatcher
    {
        void Send<TCommand>(TCommand command) where TCommand : ICommand;
        void SendWithTransaction<TCommand>(TCommand command) where TCommand : ICommand;
    }

    [AutoRegister(LifetimeScope = LifetimeScopeType.InstancePerLifetimeScope)]
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IResolver _resolver;

        public CommandDispatcher(IResolver resolver)
        {
            _resolver = resolver;
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handlerFactory = _resolver.Resolve<ICommandHandlerFactory<TCommand>>();
            handlerFactory
                .CreateHandler()
                .Handle(command);            
        }

        public void SendWithTransaction<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handlerFactory = _resolver.Resolve<ICommandHandlerFactory<TCommand>>();
            handlerFactory
                .CreateTransactionalHandler()
                .Handle(command);
        }
    }
}
