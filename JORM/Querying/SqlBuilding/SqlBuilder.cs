// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Linq.Expressions;
// using System.Text;
// using System.Threading;
//
//
// namespace JORM.Querying.SqlBuilding
// {
//     public class Source
//     {
//         public SqlBuilder Builder { get; }
//     }
//
//     public class SqlBuilder
//     {
//         private readonly Source _source;
//
//         public SqlBuilder(Source source)
//         {
//             _source = source ?? throw new ArgumentNullException(nameof(source));
//         }
//
//
//     }
//
//
//     public abstract class SqlStatement
//     {
//         protected internal readonly List<Column> _selects;
//         protected internal readonly JoinTableClauseDictionary _joinTableClauses;
//         protected internal readonly List<OrderBy> _orderBys;
//         protected internal bool _distinct;
//         protected internal int? _limit;
//
//
//         internal SqlStatement()
//         {
//             _selects = new List<Column>(8);
//             _joinTableClauses = new JoinTableClauseDictionary();
//             _orderBys = new List<OrderBy>(0);
//             _distinct = true;
//             _limit = null;
//         }
//
//         internal SqlStatement(SqlStatement statement)
//         {
//             _selects = statement._selects;
//             _joinTableClauses = statement._joinTableClauses;
//             _orderBys = statement._orderBys;
//             _distinct = statement._distinct;
//             _limit = statement._limit;
//         }
//     }
//     //
//     // public class SqlStatement<TBuilder> : SqlStatement, IInitialSqlBuilder<TBuilder>,
//     //                                       ISqlBuilder<TBuilder>,
//     //                                       IJoiningSqlBuilder<TBuilder>
//     //     where TBuilder : SqlStatement<TBuilder>
//     // {
//     //     protected readonly TBuilder _this;
//     //
//     //     JoinType IJoiningSqlBuilder<TBuilder>.JoinType => _joinTableClauses.GetNewest().Join;
//     //     
//     //     public SqlStatement()
//     //     {
//     //         _this = (this as TBuilder)!;
//     //     }
//     //
//     //     internal SqlStatement(TBuilder builder)
//     //         : base(builder as SqlStatement)
//     //     {
//     //
//     //     }
//     //
//     //     public TBuilder From(string tableName, string? alias = null)
//     //     {
//     //         return (TBuilder)Join(JoinType.From, tableName, alias);
//     //     }
//     //
//     //     public ITypeSqlBuilder<T> From<T>(string? alias = null)
//     //     {
//     //         throw new NotImplementedException();
//     //     }
//     //
//     //     public TBuilder Distinct(bool distinct = true)
//     //     {
//     //         _distinct = distinct;
//     //         return _this;
//     //     }
//     //
//     //     public TBuilder Limit(int? limit)
//     //     {
//     //         _limit = limit;
//     //         return _this;
//     //     }
//     //
//     //     public TBuilder Select(RawString columnName)
//     //     {
//     //         text text = columnName.Chars;
//     //         // Start processing
//     //         int start = 0;
//     //         int len;
//     //         char ch;
//     //         text tableAlias = default;
//     //         text name = default;
//     //         int i = 0;
//     //         do
//     //         {
//     //             // Skip whitespace
//     //             text.SkipWhiteSpace(ref i);
//     //             // Consume text
//     //             while (i < text.Length)
//     //             {
//     //                 ch = text[i];
//     //                 // , is the same as EOL
//     //                 if (ch == ',')
//     //                 {
//     //                     break;
//     //                 }
//     //                 // . alias separator
//     //                 else if (ch == '.')
//     //                 {
//     //                     len = i - start;
//     //                     if (len <= 0)
//     //                         throw new SqlBuildException(text.CaptureLeft(i, 10), ". separator must have a table alias before it");
//     //                     // Always trim alias up
//     //                     tableAlias = text.Slice(start, len).Trim();
//     //                     start = ++i;
//     //                 }
//     //                 else
//     //                 {
//     //                     i++;
//     //                 }
//     //             }
//     //
//     //             // No matter how we got here, we have name
//     //             len = i - start;
//     //             if (len <= 0)
//     //                 throw new SqlBuildException(text.CaptureLeft(i, 10), "There must be a valid column name");
//     //
//     //             // Lookup that table
//     //             JoinTableClauses jtc;
//     //             if (tableAlias.Length > 0)
//     //             {
//     //                 if (!_joinTableClauses.TryLookup(tableAlias, out jtc))
//     //                 {
//     //                     throw new SqlBuildException(text.CaptureLeft(i, len + 10), "Table alias does not match a known table");
//     //                 }
//     //             }
//     //             else
//     //             {
//     //                 // We use the last joined table
//     //                 if (!_joinTableClauses.TryGetNewest(out jtc))
//     //                 {
//     //                     throw new SqlBuildException(text.CaptureLeft(i, len + 10), "No tables have been Joined");
//     //                 }
//     //             }
//     //
//     //             // Always trim name up
//     //             name = text.Slice(start, len).Trim();
//     //             // Add this column
//     //             _selects.Add(new Column(jtc.Table, name));
//     //             // Move forward
//     //             start = ++i;
//     //             // While we have more (after a comma)
//     //         } while (i < text.Length);
//     //
//     //         // Done processing
//     //         return _this;
//     //     }
//     //
//     //     public TBuilder Select(FormattableString columnNames)
//     //     {
//     //         throw new NotImplementedException();
//     //     }
//     //
//     //     public TBuilder Select(Expression<Func<FormattableString>> columnNames)
//     //     {
//     //         var lambda = columnNames.Body as LambdaExpression;
//     //         throw new NotImplementedException();
//     //     }
//     //
//     //     public TBuilder Select(params RawString[] columnNames)
//     //     {
//     //         for (var i = 0; i < columnNames.Length; i++)
//     //         {
//     //             Select(columnNames[i]);
//     //         }
//     //         return _this;
//     //     }
//     //
//     //     public TBuilder Select(IEnumerable<string> columnNames)
//     //     {
//     //         foreach (var columnName in columnNames)
//     //         {
//     //             Select(columnName);
//     //         }
//     //         return _this;
//     //     }
//     //
//     //     public TBuilder SelectAll()
//     //     {
//     //         // We use the last joined table
//     //         if (!_joinTableClauses.TryGetNewest(out var jtc))
//     //         {
//     //             throw new SqlBuildException("No tables have been Joined");
//     //         }
//     //         _selects.Add(new Column(jtc.Table, "*"));
//     //         return _this;
//     //     }
//     //
//     //   
//     //     public IJoiningSqlBuilder<TBuilder> Join(JoinType join, string tableName, string? alias = null)
//     //     {
//     //         text name = ((text)tableName).Trim();
//     //         if (name.Length == 0)
//     //             throw new SqlBuildException(tableName, "Invalid Table Name");
//     //         text a = ((text)alias).Trim();
//     //         
//     //         _joinTableClauses.Register(join, name, a);
//     //         return _this;
//     //     }
//     //
//     //     public IJoiningSqlBuilder<TBuilder> InnerJoin(string tableName, string? alias = null)
//     //     {
//     //         return Join(JoinType.Inner, tableName, alias);
//     //     }
//     //
//     //     public IJoiningSqlBuilder<TBuilder> LeftJoin(string tableName, string? alias = null)
//     //     {
//     //         return Join(JoinType.Left, tableName, alias);
//     //     }
//     //
//     //     public IJoiningSqlBuilder<TBuilder> RightJoin(string tableName, string? alias = null)
//     //     {
//     //         return Join(JoinType.Right, tableName, alias);
//     //     }
//     //
//     //     public IJoiningSqlBuilder<TBuilder> FullJoin(string tableName, string? alias = null)
//     //     {
//     //         return Join(JoinType.Full, tableName, alias);
//     //     }
//     //
//     //     public ITypeJoiningSqlBuilder<T> Join<T>(JoinType @join, string? alias = null)
//     //     {
//     //         throw new NotImplementedException();
//     //     }
//     //
//     //     public ITypeJoiningSqlBuilder<T> InnerJoin<T>(string? alias = null)
//     //     {
//     //         throw new NotImplementedException();
//     //     }
//     //
//     //     public ITypeJoiningSqlBuilder<T> LeftJoin<T>(string? alias = null)
//     //     {
//     //         throw new NotImplementedException();
//     //     }
//     //
//     //     public ITypeJoiningSqlBuilder<T> RightJoin<T>(string? alias = null)
//     //     {
//     //         throw new NotImplementedException();
//     //     }
//     //
//     //     public TBuilder Where(RawString itemPredicate, params object?[] args)
//     //     {
//     //         var jtc = _joinTableClauses.GetFrom();
//     //         var clause = new Clause(itemPredicate, args);
//     //         jtc.Clauses.Add(clause);
//     //         return _this;
//     //     }
//     //
//     //     public TBuilder Where(FormattableString itemPredicate)
//     //     {
//     //         var jtc = _joinTableClauses.GetFrom();
//     //         var clause = new Clause(itemPredicate);
//     //         jtc.Clauses.Add(clause);
//     //         return _this;
//     //     }
//     //
//     //     public TBuilder On(RawString itemPredicate, params object?[] args)
//     //     {
//     //         var jtc = _joinTableClauses.GetNewest();
//     //         var clause = new Clause(itemPredicate, args);
//     //         jtc.Clauses.Add(clause);
//     //         return _this;
//     //     }
//     //
//     //     public TBuilder On(FormattableString itemPredicate)
//     //     {
//     //         var jtc = _joinTableClauses.GetNewest();
//     //         var clause = new Clause(itemPredicate);
//     //         jtc.Clauses.Add(clause);
//     //         return _this;
//     //     }
//     // }
//
//     public record OrderBy(Column Column, OrderDirection Direction);
// }
