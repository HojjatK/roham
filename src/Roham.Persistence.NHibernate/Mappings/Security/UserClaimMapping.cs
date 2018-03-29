namespace Roham.Domain.Entities.Security
{
    public class UserClaimMapping : IdentifiableMap<UserClaim>
    {
        public UserClaimMapping()
        {
            Map(x => x.ClaimType);
            Map(x => x.ClaimValue);

            References(x => x.User, MappingNames.RefId<User>());
        }
    }
}
