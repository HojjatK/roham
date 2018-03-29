namespace Roham.Domain.Entities.Sites
{
    public class RedirectMapping : AggregateRootMap<Redirect>
    {
        public RedirectMapping()
        {
            Map(x => x.From);
            Map(x => x.To);
            Map(x => x.Timestamp);
        }
    }
}
