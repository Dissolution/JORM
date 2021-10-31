using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JORM.Querying.SqlBuilding
{
    public class Source
    {
        public SqlBuilder Builder { get; }
    }

    public class SqlStatement
    {
        
    }

    public class SqlBuilder
    {
        private readonly Source _source;

        public SqlBuilder(Source source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }


    }
}
