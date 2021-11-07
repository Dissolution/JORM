using System;

namespace JORM.Querying.SqlBuilding
{
    public interface IJoiningSqlBuilder<TBuilder>
        where TBuilder : ISqlBuilder<TBuilder>
    {
        JoinType JoinType { get; }

        public TBuilder On(RawString itemPredicate, params object?[] args);

        public TBuilder On(FormattableString itemPredicate);
    }
}