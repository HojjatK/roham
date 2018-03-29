namespace Roham.Lib.Domain
{
    public abstract class VersionedAggregateRoot : AggregateRoot
    {
        public virtual int Version { get; set; }
    }
}
