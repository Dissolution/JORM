using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace JORM.Querying.SqlBuilding
{
    public enum JoinType
    {
        Inner,
        Left,
        Right,
        Full,
    }

    public enum OrderDirection
    {
        Ascending,
        Descending,
    }

    public interface IInitialSqlBuilder<TBuilder>
        where TBuilder : ISqlBuilder<TBuilder>
    {
        TBuilder From(string tableName, string? alias = null);
        ITypeSqlBuilder<T> From<T>(string? alias = null);
    }

    public interface ISqlBuilder<TBuilder>
        where TBuilder : ISqlBuilder<TBuilder>
    {
        TBuilder Select(RawString columnName);
        TBuilder Select(FormattableString columnNames);
        TBuilder Select(params RawString[] columnNames);
        TBuilder Select(IEnumerable<string> columnNames);
        TBuilder SelectAll();

        TBuilder Distinct(bool distinct = true);

        IJoiningSqlBuilder<TBuilder> Join(JoinType type, string tableName, string? alias = null);
        IJoiningSqlBuilder<TBuilder> InnerJoin(string tableName, string? alias = null);
        IJoiningSqlBuilder<TBuilder> LeftJoin(string tableName, string? alias = null);
        IJoiningSqlBuilder<TBuilder> RightJoin(string tableName, string? alias = null);
        IJoiningSqlBuilder<TBuilder> FullJoin(string tableName, string? alias = null);

        ITypeJoiningSqlBuilder<T> Join<T>(JoinType type, string? alias = null);
        ITypeJoiningSqlBuilder<T> InnerJoin<T>(string? alias = null);
        ITypeJoiningSqlBuilder<T> LeftJoin<T>(string? alias = null);
        ITypeJoiningSqlBuilder<T> RightJoin<T>(string? alias = null);

        TBuilder Where(RawString itemPredicate, params object?[] args);
        TBuilder Where(FormattableString itemPredicate);

        TBuilder OrderBy(RawString columnName, OrderDirection direction = OrderDirection.Ascending);
        TBuilder OrderBy(FormattableString columnName, OrderDirection direction = OrderDirection.Ascending);

        TBuilder Limit(int limit);

    }


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


    public interface IJoiningSqlBuilder<TBuilder>
        where TBuilder : ISqlBuilder<TBuilder>
    {
        JoinType JoinType { get; }

        TBuilder On(string boolExpr);
    }

    public interface ITypeJoiningSqlBuilder<TBuilder, T> : IJoiningSqlBuilder<TBuilder>
        where TBuilder : ITypeSqlBuilder<TBuilder, T>
    {
        TBuilder On(Expression<Func<T, bool>> itemPredicate);
    }

    public interface ITypeJoiningSqlBuilder<T> : ITypeJoiningSqlBuilder<ITypeSqlBuilder<T>, T>
    {

    }

    public interface IJoiningSqlBuilder<TBuilder, T1, T2> : IJoiningSqlBuilder<TBuilder>
        where TBuilder : ISqlBuilder<TBuilder>
    {
        ITypeSqlBuilder<T2> On(Expression<Func<T1, T2, bool>> matchPredicate);
    }


    public interface IInitialSqlBuilder : IInitialSqlBuilder<ISqlBuilder>
    {

    }

    public interface ISqlBuilder : ISqlBuilder<ISqlBuilder>
    {

    }

    public class SqlBuilder<TBuilder> : ISqlBuilder<TBuilder>
        where TBuilder : ISqlBuilder<TBuilder>
    {
        protected readonly List<Table> _tables;
    }

    public static class ScratchPad
    {
        public class Entity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Thing
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        static ScratchPad()
        {
            Entity entity = new Entity();
            IInitialSqlBuilder start = default!;
            start.From("entities", "e")
                .Select("e.Id", "e.Name")
                .Where("e.Id >= ?", 0)
                .Where($"e.Id >= {0}");

        }
    }

    public record Table(string Name, string? Alias);

    public record Column(Table Table, string Name);

    public interface ISqlDialect
    {
        bool CaseSensitive { get; set; }
    }

    public interface ITableResolver
    {
        Table GetTable(string name, string? alias = null);
        Table GetTable<TEntity>();
        Table GetTable(Type entityType);
    }

    public interface IColumnResolver
    {
        Column GetColumn(Table table, string name);
        Column GetColumn<TEntity, TColumn>(Expression<Func<TEntity, TColumn>> columnExpression);
        Column GetColumn<TEntity>(Expression<Func<TEntity, object?>> columnExpression);
    }

    public interface IEntityMap : IEquatable<IEntityMap>
    {
        Type EntityType { get; }

    }

    public class TableDictionary
    {
        

        private readonly List<Table> _tables;
        private readonly StringComparison _nameComparison;

        public TableDictionary(StringComparison comparison = StringComparison.CurrentCulture)
        {
            _tables = new List<Table>(8);
            _nameComparison = comparison;
        }

        public Table Register(string name, string? alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                foreach (var tbl in _tables)
                {
                    if (string.Equals(tbl.Name, name, _nameComparison))
                    {
                        return tbl;
                    }
                }
            }
            else
            {
                foreach (var tbl in _tables)
                {
                    if (string.Equals(tbl.Name, name, _nameComparison) &&
                        string.Equals(tbl.Alias, alias, _nameComparison))
                    {
                        return tbl;
                    }
                }
            }
            var table = new Table(name, alias);
            _tables.Add(table);
            return table;
        }

        public Table? Lookup(string nameOrAlias)
        {
            foreach (var table in _tables)
            {
                if (string.Equals(table.Name, nameOrAlias, _nameComparison))
                    return table;
                if (!string.IsNullOrWhiteSpace(table.Alias) &&
                    string.Equals(table.Alias, nameOrAlias, _nameComparison))
                    return table;
            }
            return null;
        }
    }
}
