using FluentNHibernate;

namespace Roham.Domain.Entities.Security
{
    public class AppFunctionMapping : AggregateRootMap<AppFunction>
    {
        public AppFunctionMapping()
        {
            Map(x => x.Key);
            Map(x => x.Name);
            Map(x => x.Title);
            Map(x => x.Description);
            References(x => x.Parent, MappingNames.ParentId);

            HasManyToMany<Role>(Reveal.Member<AppFunction>(AppFunction.NameOfRoles))
                .AsSet()
                .Table(MappingNames.AppFunctionRoleMapTableName)
                .ParentKeyColumn(MappingNames.RefId<AppFunction>())
                .ChildKeyColumn(MappingNames.RefId<Role>())
                .Inverse()
                .Cascade.None();
        }
    }
}
