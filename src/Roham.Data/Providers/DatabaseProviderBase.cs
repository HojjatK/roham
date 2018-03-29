using System;
using System.Data;

namespace Roham.Data.Providers
{
    public abstract class DatabaseProviderBase
    {
        protected bool CheckDatabaseConnection(Func<IDbConnection> connectionFactory, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                using (var conn = connectionFactory())
                {
                    conn.Open();

                    new AdhocSqlRunner(() => conn.CreateCommand()).ExecuteScalar("select 1");
                }
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }
    }
}
