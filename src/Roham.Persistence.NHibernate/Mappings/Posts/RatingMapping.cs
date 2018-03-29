namespace Roham.Domain.Entities.Posts
{
    public class RatingMapping : IdentifiableMap<Rating>
    {
        public RatingMapping()
        {
            Map(x => x.Rate);
            Map(x => x.RatedDate);
            Map(x => x.UserIdentity);
            Map(x => x.UserEmail);

            References(x => x.Post, MappingNames.RefId<Post>());
        }
    }
}
