using NHibernate;
using FluentNHibernate.Mapping;

namespace Roham.Domain.Entities.Filters
{
    public class RevisionFilter : FilterDefinition
    {
        public RevisionFilter()
        {
            WithName("RevisionFilter").AddParameter("revisionNumber", NHibernateUtil.Int32);
        }
    }
}
