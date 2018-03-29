namespace Roham.Lib.Domain.CQS.Command
{
    public interface ICommandAuthoriser
    {
        void Authorize(ISecureCommand command);
    }
}
