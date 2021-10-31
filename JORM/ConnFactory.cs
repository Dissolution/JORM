using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace JORM
{
    public class ConnFactory
    {
        protected readonly ILogger _logger;
        protected readonly Func<DbConnection> _createConnection;

        public ConnFactory(ILogger logger,
            Func<DbConnection> createConnection)
        {
            _logger = logger ?? NullLogger.Instance;
            _createConnection = createConnection ?? throw new ArgumentNullException(nameof(createConnection));
        }
        
        public virtual DbConnection GetOpenConnection()
        {
            try
            {
                var conn = _createConnection();
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create new DbConnection");
                throw;
            }
        }

        public virtual async Task<DbConnection> GetOpenConnectionAsync(CancellationToken token = default)
        {
            try
            {
                var conn = _createConnection();
                await conn.OpenAsync(token);
                return conn;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create new DbConnection");
                throw;
            }
        }
    }
}
