namespace Roham.Domain.Entities.Security
{
    public class UserLoginMapping : IdentifiableMap<UserLogin>
    {
        public UserLoginMapping()
        {
            Map(x => x.LoginProvider);
            Map(x => x.ProviderKey);

            References(x => x.User, MappingNames.RefId<User>());
        }
    }
}
