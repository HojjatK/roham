using FluentNHibernate;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Jobs;
using Roham.Domain.Entities.Sites;
using Roham.Domain.Entities.Parties;

namespace Roham.Domain.Entities.Security
{
    public class UserMapping : AggregateRootMap<User>
    {
        public UserMapping()
        {
            Map(x => x.UserName);
            Map(x => x.Email);
            Map(x => x.EmailConfirm);
            Map(x => x.PasswordHashAlgorithm);
            Map(x => x.PasswordHash);
            Map(x => x.SecurityStamp);
            Map(x => x.PhoneNumber);
            Map(x => x.PhoneNumberConfirmed);
            Map(x => x.TwoFactorEnabled);
            Map(x => x.LockoutEndDateUtc);
            Map(x => x.LockoutEnabled);
            Map(x => x.AccessFailedCount);
            Map(x => x.IsSystemUser);
            Map(x => x.Status);
            Map(x => x.StatusReason);

            References(x => x.Party, MappingNames.RefId<Party>());

            HasMany(x => x.Permissions)
                .AsSet()
                .KeyColumn(MappingNames.RefId<User>())
                .Inverse()
                .LazyLoad()
                .Cascade.All();

            HasManyToMany<Site>(Reveal.Member<User>(User.NameOfUserSites))
                .AsSet()
                .Table(MappingNames.SiteUserMapTableName)
                .ParentKeyColumn(MappingNames.RefId<User>())
                .ChildKeyColumn(MappingNames.RefId<Site>())
                .Inverse()
                .Cascade.None();

            HasManyToMany(x => x.Roles)
                .AsSet()
                .Table(MappingNames.UserRoleMapTableName)
                .ParentKeyColumn(MappingNames.RefId<User>())
                .ChildKeyColumn(MappingNames.RefId<Role>())
                .Cascade.None();

            HasMany(x => x.UserClaims)
               .AsSet()
               .Table(nameof(UserClaim))
               .KeyColumn(MappingNames.RefId<User>())
               .Inverse()
               .Cascade.All();

            HasMany(x => x.UserLogins)
               .AsSet()
               .Table(nameof(UserLogin))
               .KeyColumn(MappingNames.RefId<User>())
               .Inverse()
               .Cascade.All();

            HasMany<UserSession>(Reveal.Member<User>(User.NameOfUserSessions))
               .AsSet()
               .Table(nameof(UserSession))
               .KeyColumn(MappingNames.RefId<User>())
               .Inverse()
               .Cascade.All();

            HasMany<PostRevision>(Reveal.Member<User>(User.NameOfPostRevisions))
                .AsSet()
                .Table(nameof(PostRevision))
                .KeyColumn(MappingNames.OwnerId)
                .Inverse()
                .Cascade.All();

            HasMany<Job>(Reveal.Member<User>(User.NameOfJobs))
                .AsSet()
                .Table(nameof(Job))
                .KeyColumn(MappingNames.OwnerId)
                .Inverse()
                .Cascade.All();
        }
    }
}
