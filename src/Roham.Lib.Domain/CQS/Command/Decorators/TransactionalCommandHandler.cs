using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;

namespace Roham.Lib.Domain.CQS.Command.Decorators
{
    [AutoRegister]
    public sealed class TransactionalCommandHandler<TCommand> : ICommandHandler<TCommand>
       where TCommand : ICommand
    {
        private readonly IPersistenceUnitOfWorkFactory _uowFactory;
        private readonly ICommandHandler<TCommand> _decorated;

        public TransactionalCommandHandler(
            IPersistenceUnitOfWorkFactory uowFactory,
            ICommandHandler<TCommand> decorated)
        {
            _uowFactory = uowFactory;
            _decorated = decorated;
        }

        public void Handle(TCommand command)
        {
            using (var uow = _uowFactory.CreateWithTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                _decorated.Handle(command);

                uow.Complete();
            }
        }
    }
}
