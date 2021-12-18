// using System;
// using System.Linq.Expressions;
//
// namespace JORM.Querying.SqlBuilding
// {
//     public interface IColumnResolver
//     {
//         Column GetColumn(Table table, string name);
//         Column GetColumn<TEntity, TColumn>(Expression<Func<TEntity, TColumn>> columnExpression);
//         Column GetColumn<TEntity>(Expression<Func<TEntity, object?>> columnExpression);
//     }
// }