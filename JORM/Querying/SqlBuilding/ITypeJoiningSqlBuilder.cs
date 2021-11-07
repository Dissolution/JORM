using System;
using System.Linq.Expressions;

namespace JORM.Querying.SqlBuilding
{
    public interface ITypeJoiningSqlBuilder<TBuilder, T> : IJoiningSqlBuilder<TBuilder>
        where TBuilder : ITypeSqlBuilder<TBuilder, T>
    {
        TBuilder On(Expression<Func<T, bool>> itemPredicate);
    }
}