using System;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Logger;
using Roham.Lib.Domain.Persistence;

namespace Roham.Lib.Domain.CQS.Command
{
    public abstract class AbstractCommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : AbstractCommand
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<AbstractCommandHandler<TCommand>>();
        private readonly Func<IPersistenceUnitOfWorkFactory> _uowFactoryResolver;

        protected AbstractCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver)
        {
            _uowFactoryResolver = uowFactoryResolver;
        }

        public void Handle(TCommand command)
        {
            Log.DebugMethodParams(() => command);
            Objects.Requires<ArgumentNullException>(command != null);

            string validationErrors;
            var isCommandValid = command.TryValidate(out validationErrors);
            Objects.Requires(isCommandValid, () => new ValidationException(validationErrors));

            OnHandle(command);
        }

        protected IPersistenceUnitOfWorkFactory UowFactory => _uowFactoryResolver();

        protected abstract void OnHandle(TCommand command);
    }
}
