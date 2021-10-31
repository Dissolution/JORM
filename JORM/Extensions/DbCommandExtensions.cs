using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace JORM.Extensions
{
    public static class DbCommandExtensions
    {
        public static TCommand AddParameter<TCommand>(this TCommand command, string name, object? value)
            where TCommand : DbCommand
        {
            var param = command.CreateParameter();
            param.ParameterName = name ?? string.Empty;
            param.Value = value;
            command.Parameters.Add(param);
            return command;
        }
    }
}
