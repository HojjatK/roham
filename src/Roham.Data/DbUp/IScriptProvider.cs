using System.Collections.Generic;

namespace Roham.Data.DbUp
{
    public interface IScriptProvider
    {
        IEnumerable<SqlScript> GetScripts();
    }
}
