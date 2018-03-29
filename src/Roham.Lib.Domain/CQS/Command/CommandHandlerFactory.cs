using Roham.Lib.Domain.CQS.Command.Decorators;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;

namespace Roham.Lib.Domain.CQS.Command
{
    public interface ICommandHandlerFactory<TCommand>
        where TCommand : ICommand
    {
        ICommandHandler<TCommand> CreateHandler();
        ICommandHandler<TCommand> CreateTransactionalHandler();
    }

    [AutoRegister(LifetimeScope = LifetimeScopeType.InstancePerLifetimeScope)]
    public class CommandHandlerFactory<TCommand> : ICommandHandlerFactory<TCommand>
        where TCommand : ICommand
    {
        private readonly ILifetimeScope _lifetimeScope;

        public CommandHandlerFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public ICommandHandler<TCommand> CreateHandler()
        {
            var handler = _lifetimeScope.Resolve<ICommandHandler<TCommand>>();
            if (typeof(ISecureCommand).IsAssignableFrom(typeof(TCommand)))
            {
                var authorizer = _lifetimeScope.Resolve<ICommandAuthoriser>();
                return new DeadlockRetryCommandHandler<TCommand>(
                    new SecurableCommandHandler<TCommand>(authorizer, handler));
            }
            return new DeadlockRetryCommandHandler<TCommand>(handler);
        }

        public ICommandHandler<TCommand> CreateTransactionalHandler()
        {
            var uowFactory = _lifetimeScope.Resolve<IPersistenceUnitOfWorkFactory>();
            var handler = _lifetimeScope.Resolve<ICommandHandler<TCommand>>();

            if (typeof(ISecureCommand).IsAssignableFrom(typeof(TCommand)))
            {
                var authorizer = _lifetimeScope.Resolve<ICommandAuthoriser>();
                return new DeadlockRetryCommandHandler<TCommand>(
                    new TransactionalCommandHandler<TCommand>(uowFactory,
                       new SecurableCommandHandler<TCommand>(authorizer, handler)));
            }
            return new DeadlockRetryCommandHandler<TCommand>(
                new TransactionalCommandHandler<TCommand>(uowFactory, handler));
        }
    }
}
