using Roham.Lib.Ioc;

namespace Roham.Lib.Domain.CQS.Command.Decorators
{
    [AutoRegister]
    public class SecurableCommandHandler<TCommand> : ICommandHandler<TCommand> 
        where TCommand : ICommand
    {   
        private readonly ICommandAuthoriser _authorizer;
        private readonly ICommandHandler<TCommand> _decorated;

        public SecurableCommandHandler(
            ICommandAuthoriser authorizer,
            ICommandHandler<TCommand> decorated)
        {
            _authorizer = authorizer;
            _decorated = decorated;
        }

        public void Handle(TCommand command)
        {
            if (command is ISecureCommand)
            {
                _authorizer.Authorize(command as ISecureCommand);
            }
            _decorated.Handle(command);
        }
    }
}
