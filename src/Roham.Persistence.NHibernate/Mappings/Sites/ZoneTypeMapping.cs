using FluentNHibernate;

namespace Roham.Domain.Entities.Sites
{
    //public class ZoneTypeMapping : AggregateRootMap<ZoneType>
    //{
    //    public ZoneTypeMapping()
    //    {
    //        Map(x => x.Name);
    //        Map(x => x.Code);
    //        Map(x => x.Description);

    //        HasMany<Zone>(Reveal.Member<ZoneType>(ZoneType.NameOfZones))
    //           .AsSet()
    //           .KeyColumn(MappingNames.RefId<ZoneType>())
    //           .Inverse()
    //           .LazyLoad()
    //           .Cascade.All();
    //    }
    //}
}
