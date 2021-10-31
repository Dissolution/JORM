using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
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

    // public class SqlBuilder<TBuilder> : ISqlBuilder<TBuilder>
    //     where TBuilder : ISqlBuilder<TBuilder>
    // {
    //     protected readonly TableDictionary _tables;
    //     protected readonly List<Column> _selects;
    // }

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

    public class Table
    {
        public string Name { get; set; }
        public string? Alias { get; set; }

        public Table(string name, string? alias = null)
        {
            this.Name = name;
            this.Alias = alias;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Alias))
                return Name;
            return $"{Name} {Alias}";
        }
    }

    public class EntityTable : Table
    {
        public Type EntityType { get; }

        public EntityTable(Type entityType, string? alias = null)
            : base(entityType.Name, alias)
        {
            this.EntityType = entityType;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Alias))
                return $"[{EntityType.FullName}]";
            return $"[{EntityType.FullName}] {Alias}";
        }
    }

    public class Column
    {
        public Table Table { get; set; }
        public string Name { get; set; }

        public Column(Table table, string name)
        {
            this.Table = table;
            this.Name = name;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Table.Alias))
            {
                return $"{Table.Name}.{Name}";
            }
            else
            {
                return $"{Table.Alias}.{Name}";
            }
        }
    }

    public class EntityColumn : Column
    {
        public PropertyInfo ColumnProperty { get; }

        public EntityColumn(PropertyInfo property,
                            Table table)
            : base(table, property.Name)
        {
            this.ColumnProperty = property;
        }
    }

    public static class TextHelper
    {
        public static void ConsumeWhitespace(this ReadOnlySpan<char> text, ref int index)
        {
            if (index < 0)
            {
                index = -1;
            }
            else if (index >= text.Length)
            {
                index = text.Length;
            }
            else
            {
                while (char.IsWhiteSpace(text[index]))
                {
                    index++;
                    if (index == text.Length)
                        return;
                }
            }
        }

        public static ReadOnlySpan<char> ConsumeDigits(this ReadOnlySpan<char> text, ref int index)
        {
            if (index < 0)
            {
                index = -1;
                return default;
            }
            if (index >= text.Length)
            {
                index = text.Length;
                return default;
            }

            int start = index;
            while (char.IsDigit(text[index]))
            {
                index++;
                if (index == text.Length)
                    break;
            }
            return text.Slice(start, index - start);
        }

        public static string CaptureLeft(this ReadOnlySpan<char> text, int index, int count)
        {
            int left = index - count;
            ReadOnlySpan<char> slice;
            if (left < 0)
            {
                slice = text.Slice(0, count + left);
            }
            else
            {
                slice = text.Slice(left, count);
            }
            return new string(slice);
        }
    }

    public class Clause
    {
        public static string Process(ReadOnlySpan<char> format, params object?[] args)
        {
            // Clean up
            format = format.Trim();
            var formatLen = format.Length;
            var argsCount = args.Length;
            var statement = new StringBuilder(format.Length + (2 * argsCount));
            int i = 0;
            int a = 0;
            while (i < formatLen)
            {
                char c = format[i];
                i += 1;
                if (c == '?')
                {
                    if (i >= formatLen)
                    {
                        if (a >= argsCount)
                        {
                            i--;
                            throw new ArgumentException($"{format.CaptureLeft(i, 10)}\"[{i}]: The final `?` argument placeholder wants index {a}; no argument with that index exist.", nameof(format));
                        }
                        statement.Append('?').Append(a);
                        break;
                    }  
                    if (char.IsWhiteSpace(c = format[i]))
                    {
                        if (a >= argsCount)
                        {
                            i--;
                            throw new ArgumentException($"{format.CaptureLeft(i, 10)}\"[{i}]: The `?` argument placeholder wants index {a}; no argument with that index exist.", nameof(format));
                        }
                        statement.Append('?').Append(a).Append(c);
                        i++;
                        a++;
                        continue;
                    }
                    if (char.IsDigit(format[i]))
                    {
                        var digits = format.ConsumeDigits(ref i);
                        Debug.Assert(digits.Length >= 1);
                        a = int.Parse(digits);
                        if (a >= argsCount)
                        {
                            throw new ArgumentException($"{format.CaptureLeft(i, 10)}\"[{i - 1}]: The `?` argument placeholder wants index {a}; no argument with that index exist.", nameof(format));
                        }
                        statement.Append('?').Append(a);
                        i++;
                        a++;
                        continue;
                    }

                    throw new ArgumentException($"{format.CaptureLeft(i, 10)}\"[{i - 1}]: The `?` argument placeholder is followed by an illegal character", nameof(format));
                }

                if (c == '{')
                {
                    if (i >= formatLen)
                    {
                        throw new ArgumentException($"{format.CaptureLeft(i, 10)}\"[{i}]: The argument placeholder start `{{` has nothing after it", nameof(format));
                    }
                    // Escaped?
                    if (format[i] == '{')
                    {
                        statement.Append('{');
                        i++;
                        continue;
                    }
                    format.ConsumeWhitespace(ref i);
                    if (char.IsDigit(format[i]))
                    {
                        var slice = format.ConsumeDigits(ref i);
                        format.ConsumeWhitespace(ref i);
                        Debug.Assert(slice.Length >= 1);
                        if (i >= formatLen)
                        {
                            throw new ArgumentException($"{format.CaptureLeft(i, 10)}\"[{i}]: The argument placeholder is missing the closing brace`}}`", nameof(format));
                        }
                        if (format[i] == '}')
                        {
                            a = int.Parse(slice);
                            if (a >= argsCount)
                            {
                                throw new ArgumentException($"{format.CaptureLeft(i, 10)}\"[{i - 1}]: The `{{}}` argument placeholder wants index {a}; no argument with that index exist.", nameof(format));
                            }
                            statement.Append('?').Append(a);
                            i++;
                            a++;
                            continue;
                        }

                        throw new ArgumentException($"{format.CaptureLeft(i, 10)}\"[{i}]: The argument placeholder is missing the closing brace`}}`", nameof(format));
                    }

                    throw new ArgumentException("Invalid character after { argument placeholder", nameof(format));
                }

                if (c == '}')
                {
                    if (i >= formatLen)
                    {
                        throw new ArgumentException($"{format.CaptureLeft(i, 10)}\"[{i}]: The argument placeholder end `}}` has no start", nameof(format));
                    }
                    
                    if (format[i] == '}')
                    {
                        statement.Append('}');
                        i++;
                        continue;
                    }

                    throw new ArgumentException($"{format.CaptureLeft(i, 10)}\"[{i}]: The argument placeholder end `}}` has no start", nameof(format));
                }

                // This character we just append
                statement.Append(c);
            }

            return statement.ToString();
        }

        public string Statement { get; set; }
        public object?[] Arguments { get; set; }

        public Clause(RawString statement, params object?[] args)
        {
            this.Statement = Process(statement.String);
            this.Arguments = args;
        }

        public Clause(FormattableString statement)
        {
            this.Statement = Process(statement.Format);
            this.Arguments = statement.GetArguments();
        }
    }

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
        

        private readonly List<(JoinType Join, Table Table)> _joinedTables;
        private readonly StringComparison _nameComparison;

        public TableDictionary(StringComparison comparison = StringComparison.CurrentCulture)
        {
            _joinedTables = new List<(JoinType, Table)>(8);
            _nameComparison = comparison;
        }

        public Table Register(JoinType join, string name, string? alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                foreach (var (exJoin, exTable) in _joinedTables)
                {
                    if (string.Equals(exTable.Name, name, _nameComparison))
                    {
                        return table1;
                    }
                }
            }
            else
            {
                foreach (var tbl in _joinedTables)
                {
                    if (string.Equals(tbl.Name, name, _nameComparison) &&
                        string.Equals(tbl.Alias, alias, _nameComparison))
                    {
                        return tbl;
                    }
                }
            }

            {
                var table = new Table(name, alias);
                _joinedTables.Add(table);
                return table;
            }
        }

        public Table? Lookup(string nameOrAlias)
        {
            foreach (var table in _joinedTables)
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
