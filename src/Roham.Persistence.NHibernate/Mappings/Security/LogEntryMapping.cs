using Roham.Domain.Entities;
using Roham.Domain.Entities.Security;

namespace Roham.Persistence.NHibernate.Mappings.Security
{
    public class LogEntryMapping : AggregateRootMap<LogEntry>
    {
        public LogEntryMapping()
        {
            Map(x => x.Timestamp);
            Map(x => x.Level);
            Map(x => x.Message);
            Map(x => x.LoggerName);
            Map(x => x.Thread);
            Map(x => x.ProcessUser);
            Map(x => x.SessionToken);
            Map(x => x.SessionUser);
            Map(x => x.StackTrace);
            Map(x => x.ClientStackTrace);
            Map(x => x.Exception);
            Map(x => x.Extra);
        }
    }
}
