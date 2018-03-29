
namespace Roham.Data.DbUp.ScriptExecutor
{
    internal sealed class SqlServerCommandReader : DbCommandReader
    {
        public SqlServerCommandReader(string sqlText) : base(sqlText) { }
    }
}
