using System;
using System.Data;

namespace Roham.Data.DbUp.ScriptExecutor
{
    internal class ConnectionFactory
    {
        public ConnectionFactory(
            Func<IDbConnection> connectionFactory,
            Func<IDbConnection, IDbTransaction, IDbCommand> commandFactory)
        {

            this.GetConnection = connectionFactory;
            this.GetCommand = commandFactory;
        }

        public IDbTransaction CurrentTransaction { get; private set; }
        public void SetCurrentTransaction(IDbTransaction tranx)
        {
            CurrentTransaction = tranx;
        }

        public Func<IDbConnection> GetConnection { get; private set; }
        public Func<IDbConnection, IDbTransaction, IDbCommand> GetCommand { get; private set; }
    }
}
