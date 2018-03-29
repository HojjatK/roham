using System;
using System.Collections.Generic;

namespace Roham.Data.DbUp
{
    public class UpgradeResult
    {
        public UpgradeResult(List<SqlScript> executed, bool successful, Exception error)
        {
            Scripts = executed;
            Successful = successful;
            Error = error;
        }

        public IEnumerable<SqlScript> Scripts { get; }        
        public bool Successful { get; }
        public Exception Error { get; }
    }
}
