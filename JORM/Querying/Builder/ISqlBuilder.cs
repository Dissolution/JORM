using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JORM.Querying.SqlBuilding;

namespace JORM.Querying.Builder;

/* https://en.wikibooks.org/wiki/Structured_Query_Language
 *
 *
 */


public interface ISelectQueryBuilder : ISelectBuilder<ISelectQueryBuilder>
{

}

public interface ITypeSelectQueryBuilder<TEntity> : 
    ITypeSelectBuilder<TEntity, ITypeSelectQueryBuilder<TEntity>>,
    ISelectBuilder<ITypeSelectQueryBuilder<TEntity>>
{

}

public interface ISelectBuilder
{
    ISelectQuery Compile();
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TBuilder"></typeparam>
/// <see cref="https://en.wikibooks.org/wiki/Structured_Query_Language/SELECT:_Fundamentals"/>
public interface ISelectBuilder<TBuilder> : ISelectBuilder
    where TBuilder : class, ISelectBuilder
{
    TBuilder Distinct(bool distinct = true);

    TBuilder Select(IProjection projection);

    TBuilder Where(IWhereBuilder where);
    TBuilder Where(Action<IWhereBuilder> where);
}

public interface IWhereBuilder
{
    IWhereBuilder And(IWhereBuilder left, IWhereBuilder right);
    IWhereBuilder And(Action<IWhereBuilder> left, Action<IWhereBuilder> right);

    IWhereBuilder Or(IWhereBuilder left, IWhereBuilder right);
    IWhereBuilder Or(Action<IWhereBuilder> left, Action<IWhereBuilder> right);

    IWhereBuilder Parse(RawString sql, params object?[] args);
    IWhereBuilder Parse(FormattableString sql);
    IWhereBuilder Parse(Expression expression);
}


public interface ITypeSelectBuilder<TEntity, TBuilder> : ISelectBuilder<TBuilder>
    where TBuilder : class, ITypeSelectBuilder<TEntity, TBuilder>
{
    TBuilder Select<TProperty>(Expression<Func<TEntity, TProperty>> property);
    TBuilder Select(params Expression<Func<TEntity, object?>>[] properties);
}

public interface IPreFromSelectBuilder<TBuilder>
    where TBuilder : class, ISelectBuilder
{
    ISelectBuilder<TBuilder> From(RawString sql, Alias? alias = default);
    ISelectBuilder<TBuilder> From(FormattableString sql, Alias? alias = default);
    ISelectBuilder<TBuilder> From(Type entityType, Alias? alias = default);
    ISelectBuilder<TBuilder> From(Expression expression, Alias? alias = default);
    ITypeSelectQueryBuilder<TEntity> From<TEntity>(Alias? alias = default);
}

public static class Extensions
{

}

public sealed class TableRef
{

}

public interface IQuery
{

}

public interface ISelectQuery : IQuery, IProjection
{

}

public interface IProjection
{
    Alias Alias { get; }
}

public interface IProjectionParser
{
    IProjection Parse(SQL sql);
}

public sealed class Alias : IEquatable<Alias>, IEquatable<string>
{
    public static implicit operator Alias(string? alias) => new Alias(alias);

    public static Alias Parse(string? alias) => new Alias(alias);

    internal string? Value { get; set; }

    private Alias(string? alias)
    {
        this.Value = alias;
    }

    public bool Equals(Alias? alias)
    {
        if (alias is null)
            return Value is null;
        return string.Equals(alias.Value, this.Value);
    }

    public bool Equals(string? alias)
    {
        return string.Equals(alias, Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is string str)
            return string.Equals(str, Value);
        if (obj is Alias alias)
            return string.Equals(alias.Value, Value);
        return false;
    }

    [Obsolete("Alias is a mutable variable unsuitable for GetHashCode()")]
    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Value ?? "";
    }
}

public interface IColumn : IProjection
{
    string Name { get; }
}

public interface IAllColumns : IColumn
{
    string IColumn.Name => "*";
}

public interface IColumnFunction : IProjection
{

}

public interface IFunction : IProjection
{

}

public interface IFixedValue : IProjection
{

}

