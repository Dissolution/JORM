using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace JORM.Querying.SqlBuilding
{
    public interface ISqlBuilder<TBuilder>
        where TBuilder : ISqlBuilder<TBuilder>
    {
        TBuilder Select(RawString columnName);
        TBuilder Select(FormattableString columnNames);
        TBuilder Select(Expression<Func<FormattableString>> columnNames);
        TBuilder Select(params RawString[] columnNames);
        TBuilder Select(IEnumerable<string> columnNames);
        TBuilder SelectAll();

        TBuilder Distinct(bool distinct = true);

        IJoiningSqlBuilder<TBuilder> Join(JoinType @join, string tableName, string? alias = null);
        IJoiningSqlBuilder<TBuilder> InnerJoin(string tableName, string? alias = null);
        IJoiningSqlBuilder<TBuilder> LeftJoin(string tableName, string? alias = null);
        IJoiningSqlBuilder<TBuilder> RightJoin(string tableName, string? alias = null);
        IJoiningSqlBuilder<TBuilder> FullJoin(string tableName, string? alias = null);

        ITypeJoiningSqlBuilder<T> Join<T>(JoinType @join, string? alias = null);
        ITypeJoiningSqlBuilder<T> InnerJoin<T>(string? alias = null);
        ITypeJoiningSqlBuilder<T> LeftJoin<T>(string? alias = null);
        ITypeJoiningSqlBuilder<T> RightJoin<T>(string? alias = null);

        TBuilder Where(RawString itemPredicate, params object?[] args);
        TBuilder Where(FormattableString itemPredicate);

        TBuilder OrderBy(RawString columnName, OrderDirection direction = OrderDirection.Ascending);
        TBuilder OrderBy(FormattableString columnName, OrderDirection direction = OrderDirection.Ascending);

        TBuilder Limit(int? limit);

    }

    public static class SqlBuilderExtensions
    {
      
    }
}