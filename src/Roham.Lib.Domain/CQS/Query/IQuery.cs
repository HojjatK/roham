namespace Roham.Lib.Domain.CQS.Query
{
    public interface IQuery<out TResult>
    {
        string QueryString { get; }
    }

    public interface IPagedQuery<out TResult>
    {
        string QueryString { get; }
    }
}