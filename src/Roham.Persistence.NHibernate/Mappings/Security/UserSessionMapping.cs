namespace Roham.Domain.Entities.Security
{
    public class UserSessionMapping : AggregateRootMap<UserSession>
    {
        public UserSessionMapping()
        {
            Map(x => x.Status);
            Map(x => x.StartTimestamp);
            Map(x => x.EndTimestamp);

            References<User>(x => x.User, MappingNames.RefId<User>());
        }
    }
}
