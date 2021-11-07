using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JORM.Querying.SqlBuilding
{
    public class SqlBuildException : Exception
    {
        public SqlBuildException(string? message)
            : base(message, null)
        {

        }

        public SqlBuildException(string? message, Exception? innerException)
            : base(message, innerException)
        {

        }

        private static string GetMessage(text sql, int index, string message)
        {
            string prev = sql.CaptureLeft(index, CaptureLen);
            if (!string.IsNullOrWhiteSpace(message))
            {
                return $"\"{prev}\"<[{index}]: {message}";
            }
            return $"\"{prev}\"<[{index}]";
        }

        private const int CaptureLen = 10;

        internal SqlBuildException(text sql, int index, string message, string? memberName = null)
            : base(GetMessage(sql, index, message), 
                string.IsNullOrWhiteSpace(memberName) ? null : new ArgumentException(message, memberName))
        {

        }
    }
}
