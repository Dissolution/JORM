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
    }

}
