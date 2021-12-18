using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace JORM
{
    public class Source
    {
        public static Source<TProvider> Create<TProvider>(TProvider provider)
            where TProvider : DbProviderFactory
        {
            throw new NotImplementedException();
        }

        private readonly DbTypeMap _dbTypeMap = DbTypeMap.Default;

        protected readonly Func<DbConnection> _createConnection;

        public Source(Func<DbConnection> createConnection)
        {
            _createConnection = createConnection;
        }

        public DbConnection CreateConnection()
        {
            return _createConnection();
        }

        public DbConnection CreateOpenConnection()
        {
            var connection = _createConnection();
            connection.Open();
            return connection;
        }

        public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken token = default)
        {
            var connection = _createConnection();
            await connection.OpenAsync(token);
            return connection;
        }

        public int ExecuteNonQuery(ref InterpolatedSqlHandler sql)
        {
            string sqlTExt = sql.ToStringAndClear();
            var ps = new List<DbParam>();

            foreach (var (name, type, value) in sql.Parameters)
            {
                Debugger.Break();
            }

            using (var conn = CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql.ToStringAndClear();
                foreach (var (name, type, value) in sql.Parameters)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = name;
                    p.DbType = _dbTypeMap[type];
                    p.Value = value;
                    cmd.Parameters.Add(p);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        public async Task<int> ExecuteNonQueryAsync(FormattableString sql, CancellationToken token = default)
        {
            using (var conn = await CreateOpenConnectionAsync(token))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql.ToStringAndClear();
                foreach (var (name, type, value) in sql.Parameters)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = name;
                    p.DbType = _dbTypeMap[type];
                    p.Value = value;
                    cmd.Parameters.Add(p);
                }
                return await cmd.ExecuteNonQueryAsync(token);
            }
        }
    }


    public class Source<TProvider>
        where TProvider : DbProviderFactory
    {
       
    }

    public interface ISourceBuilder
    {
        ISourceBuilder<TProvider> Provider<TProvider>(TProvider provider)
            where TProvider : DbProviderFactory;
    }

    public interface ISourceBuilder<TProvider>
        where TProvider : DbProviderFactory
    {
        ISourceBuilder<TProvider, TConnection> Connection<TConnection>(Func<TProvider, TConnection> createConnection)
            where TConnection : DbConnection;
    }

    public interface ISourceBuilder<TProvider, TConnection>
        where TProvider : DbProviderFactory
        where TConnection : DbConnection
    {
        ISourceBuilder<TProvider, TConnection, TTransaction> Transaction<TTransaction>(Func<TConnection, TTransaction> createTransaction)
            where TTransaction : DbTransaction;
    }

    public interface ISourceBuilder<TProvider, TConnection, TTransaction>
        where TProvider : DbProviderFactory
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        ISourceBuilder<TProvider, TConnection, TTransaction, TCommand> Command<TCommand>(Func<TConnection, TCommand> createCommand)
            where TCommand : DbCommand;
    }

    public interface ISourceBuilder<TProvider, TConnection, TTransaction, TCommand>
        where TProvider : DbProviderFactory
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TCommand : DbCommand
    {
        ISourceBuilder<TProvider, TConnection, TTransaction, TCommand, TParameter> Parameter<TParameter>(Func<TCommand, TParameter> createParameter)
            where TParameter : DbParameter;
    }
    
    public interface ISourceBuilder<TProvider, TConnection, TTransaction, TCommand, TParameter>
        where TProvider : DbProviderFactory
        where TConnection : DbConnection
        where TCommand : DbCommand
        where TTransaction : DbTransaction
        where TParameter : DbParameter
    {

    }
