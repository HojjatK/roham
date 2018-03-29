using FluentNHibernate;

namespace Roham.Domain.Entities.Security
{
    public class RoleMapping : AggregateRootMap<Role>
    {
        public RoleMapping()
        {
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.IsSystemRole);
            Map(x => x.RoleType);

            HasManyToMany<User>(Reveal.Member<Role>(Role.NameOfUsers))
                .AsSet()
                .Table(MappingNames.UserRoleMapTableName)
                .ParentKeyColumn(MappingNames.RefId<Role>())
                .ChildKeyColumn(MappingNames.RefId<User>())
                .Inverse()
                .Cascade.None();

            HasManyToMany(x => x.AppFunctions)
                .AsSet()
                .Table(MappingNames.AppFunctionRoleMapTableName)
                .ParentKeyColumn(MappingNames.RefId<Role>())
                .ChildKeyColumn(MappingNames.RefId<AppFunction>())                
                .Cascade.SaveUpdate();
        }
    }
}
