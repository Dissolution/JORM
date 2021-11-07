using System;
using System.Linq.Expressions;

namespace JORM.Querying.SqlBuilding
{
    public interface ITypeSqlBuilder<TBuilder, T> : ISqlBuilder<TBuilder>
        where TBuilder : ITypeSqlBuilder<TBuilder, T>
    {
        TBuilder Select<TColumn>(Expression<Func<T, TColumn>> columnExpression);
        TBuilder Select(params Expression<Func<T, object?>>[] columnsExpression);

        new IJoiningSqlBuilder<TBuilder, T, T2> Join<T2>(JoinType type, string? alias = null);
        new IJoiningSqlBuilder<TBuilder, T, T2> InnerJoin<T2>(string? alias = null);
        new IJoiningSqlBuilder<TBuilder, T, T2> LeftJoin<T2>(string? alias = null);
        new IJoiningSqlBuilder<TBuilder, T, T2> RightJoin<T2>(string? alias = null);

        TBuilder Where(Expression<Func<T, bool>> itemPredicate);

        TBuilder OrderBy<TColumn>(Expression<Func<T, TColumn>> columnExpression,
                                  OrderDirection direction = OrderDirection.Ascending);
    }

    public interface ITypeSqlBuilder<T> : ITypeSqlBuilder<ITypeSqlBuilder<T>, T>
    {

    }


    public interface ITypeJoiningSqlBuilder<T> : ITypeJoiningSqlBuilder<ITypeSqlBuilder<T>, T>
    {

    }

    public interface IJoiningSqlBuilder<TBuilder, T1, T2> : IJoiningSqlBuilder<TBuilder>
        where TBuilder : ISqlBuilder<TBuilder>
    {
        ITypeSqlBuilder<T2> On(Expression<Func<T1, T2, bool>> matchPredicate);
    }


    public interface ISqlBuilder : ISqlBuilder<ISqlBuilder>
    {

    }

    // public class SqlBuilder<TBuilder> : ISqlBuilder<TBuilder>
    //     where TBuilder : ISqlBuilder<TBuilder>
    // {
    //     protected readonly TableDictionary _tables;
    //     protected readonly List<Column> _selects;
    // }
}
