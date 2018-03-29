using Roham.Lib.Ioc;
using System;
using System.Data.Common;
using System.Threading;

namespace Roham.Lib.Domain.CQS.Command.Decorators
{
    [AutoRegister]
    public sealed class DeadlockRetryCommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly static int TryCount = 5;
        private readonly ICommandHandler<TCommand> _decorated;

        public DeadlockRetryCommandHandler(ICommandHandler<TCommand> decorated)
        {
            _decorated = decorated;
        }

        public void Handle(TCommand command)
        {
            HandleWithCountDown(command, TryCount);
        }

        private void HandleWithCountDown(TCommand command, int count)
        {
            try
            {
                _decorated.Handle(command);
            }
            catch (Exception ex)
            {
                if (count <= 0 || !IsDeadlockException(ex))
                {
                    throw;
                }

                Thread.Sleep(300);

                HandleWithCountDown(command, count - 1);
            }
        }

        private static bool IsDeadlockException(Exception ex)
        {
            while (ex != null)
            {
                if (ex is DbException && ex.Message.Contains("deadlock"))
                {
                    return true;
                }
                ex = ex.InnerException;
            }

            return false;
        }
    }
}
