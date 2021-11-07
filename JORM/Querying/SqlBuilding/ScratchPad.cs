using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace JORM.Querying.SqlBuilding
{
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
            public Guid GUID { get; set; }
        }

        static ScratchPad()
        {
            Entity entity = new Entity();
            ISelectQueryBuilderStart start = default!;
            start.From("entities", "e")
                .Select("e.Id", "e.Name")
                .Select("e.timestamp")
                .Select($"e.{nameof(Guid)}")
                .Where("e.Id > 0")
                .Where(b => b.And(b.Parse(""), b.Parse("")))
                ;

            start.From<Entity>("e")
                .Select(e => e.Id, e => e.Name)
                .Where(b => b.Or(b.Parse(e => e.Id > 0),
                    b.And(b.Parse(e => e.Name != null), b.Parse("e.GUID <> default"))));
        }

        public interface ISelectQueryBuilderStart
        {
            ISelectQuery From(string tableName, string? alias = null);
            ISelectQuery From(Type entityType, string? alias = null);
            ISelectQuery<TEntity> From<TEntity>(string? alias = null);
        }

        public interface ISelectQueryBuilder<TBuilder>
            where TBuilder : ISelectQueryBuilder<TBuilder>
        {
            TBuilder Select(RawString columnNames);
            TBuilder Select(FormattableString columnNames);
            TBuilder Select(params RawString[] columnNames);
            TBuilder Select(IEnumerable<string> columnNames);

            TBuilder Where(RawString sql, params object?[] args);
            TBuilder Where(FormattableString sql);
            TBuilder Where(Func<IClauseBuilder, IClause> buildClause);
        }

        public interface ITypeSelectQueryBuilder<TBuilder, TEntity> : ISelectQueryBuilder<TBuilder>
            where TBuilder : ITypeSelectQueryBuilder<TBuilder, TEntity>
        {
            TBuilder Select<TColumn>(Expression<Func<TEntity, TColumn>> column);
            TBuilder Select(params Expression<Func<TEntity, object>>[] columns);

            TBuilder Where(Expression<Func<TEntity, bool>> predicate);
            TBuilder Where(Func<IClauseBuilder<TEntity>, IClause> buildClause);
        }

        public interface IClause
        {
            
        }

        public abstract class Clause
        {
            public static implicit operator Clause(FormattableString sql)
            {
                throw new NotImplementedException();
            }
        }

        public abstract class Clause<TEntity>
        {
            public static implicit operator Clause<TEntity>(Expression<Func<TEntity, bool>> predicate)
            {
                throw new NotImplementedException();
            }
        }

        public interface IClauseBuilder
        {
            IClause And(params IClause[] clauses);
            IClause Or(params IClause[] clauses);
            IClause Parse(RawString sql, params object?[] args);
            IClause Parse(FormattableString sql);
        }

        public interface IClauseBuilder<TEntity> : IClauseBuilder
        {
            IClause Parse(Expression<Func<TEntity, bool>> predicate);
        }

        public interface ISelectQuery : ISelectQueryBuilder<ISelectQuery>
        {

        }

        public interface ISelectQuery<T> : ITypeSelectQueryBuilder<ISelectQuery<T>, T>
        {

        }
    }
}