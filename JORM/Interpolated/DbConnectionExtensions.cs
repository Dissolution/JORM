

namespace JORM
{
    public static class DbConnectionExtensions
    {
        public static DbCommand CreateCommand(this DbConnection connection,
                                              ref InterpolatedSqlHandler handler)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = handler.ToStringAndClear();
            cmd.Parameters.AddRange(handler.Parameters);
            return cmd;
        }
    }
}
