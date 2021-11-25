using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using TextBuilder = System.Runtime.CompilerServices.DefaultInterpolatedStringHandler;

namespace JORM.Querying.Builder
{
    /* https://en.wikibooks.org/wiki/Structured_Query_Language
     *
     *
     */

    public interface IFluentSqlBuilder<TBuilder>
        where TBuilder : IFluentSqlBuilder<TBuilder>
    {
    }

    public interface IQueryPart : IRenderable
    {
        void ApplyTo(Func<IDataParameter> createParameter, TextBuilder commandText);
    }

    public interface ISelectQuery : IQueryPart, IRenderable
    {

    }

    public interface IFluentSelectBuilder<TBuilder>
        where TBuilder : IFluentSqlBuilder<TBuilder>
    {
        TBuilder Select(RawString projection);
        TBuilder Select(FormattableString projection);
        TBuilder Select(params RawString[] projections);
        TBuilder Select(IEnumerable<string> projections);

        TBuilder Select(Projection projection);

        TBuilder Operation(ColumnOperation operation);
    }

    public interface IRenderable
    {
        void Render(TextBuilder stringHandler);
        string ToString()
        {
            var stringHandler = new TextBuilder();
            Render(stringHandler);
            return stringHandler.ToStringAndClear();
        }
    }

    public abstract class Projection : IQueryPart
    {
        public abstract void Render(TextBuilder stringHandler);
        public abstract void ApplyTo(DbCommand command, TextBuilder commandText);
    }

    public class ColumnProjection : Projection
    {
        public TableRef Table { get; init; }
        public string Name { get; init; }
        public string? Alias { get; internal set; }

        public override void Render(DefaultInterpolatedStringHandler stringHandler)
        {
            Table.Render(stringHandler);
            stringHandler.AppendLiteral(".");
            stringHandler.AppendLiteral(Name);
            if (Alias != null)
            {
                stringHandler.AppendLiteral(" ");
                stringHandler.AppendLiteral(Alias);
            }
        }
    }

    public class ColumnOperationProjection : Projection
    {
        public ColumnOperation Operation { get; init; }
        public List<ColumnProjection> Columns { get; }

        public ColumnOperationProjection(ColumnOperation operation, IEnumerable<ColumnProjection> columns)
        {
            this.Operation = operation;
            this.Columns = new List<ColumnProjection>(columns);
            if (Columns.Count < 1)
                throw new ArgumentException("There must be at least one ColumnProjection", nameof(columns));
        }

        public override void Render(DefaultInterpolatedStringHandler stringHandler)
        {
            Operation.Render(stringHandler);
            stringHandler.AppendLiteral("(");
            for (var i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                    stringHandler.AppendLiteral(", ");
                Columns[0].Render(stringHandler);
            }
            stringHandler.AppendLiteral(")");
        }
    }

    public abstract class FunctionProjection : Projection
    {
        public abstract void Render(DefaultInterpolatedStringHandler stringHandler);
    }

    public class SelectFunctionProjection : FunctionProjection
    {
        protected ISelectQuery _selectQuery;

        public SelectFunctionProjection(ISelectQuery selectQuery)
        {
            _selectQuery = selectQuery;
        }

        public override void Render(DefaultInterpolatedStringHandler stringHandler)
        {
            stringHandler.AppendLiteral("(");
            _selectQuery.Render(stringHandler);
            stringHandler.AppendLiteral(")");
        }

        public override void ApplyTo(DbCommand command)
        {
            
        }
    }

    public class ValueProjection : Projection
    {
        internal static string GetFormat(Type type)
        {
            if (type == typeof())
        }

        public Type Type { get; init; }
        public object? Value { get; init; }

        public override void Render(DefaultInterpolatedStringHandler stringHandler)
        {
            stringHandler.AppendLiteral("'");
            if (Value is ISpanFormattable spanFormattable)
            {

                stringHandler.AppendFormatted(spanFormattable);
            }
        }
    }

    public class ColumnOperation : IRenderable
    {
        public static ColumnOperation Count { get; }
        public static ColumnOperation Max { get; }
        public static ColumnOperation Min { get; }
        public static ColumnOperation Sum { get; }
        public static ColumnOperation Avg { get; }
        public static ColumnOperation Concat { get; }

        protected string _name;

        protected ColumnOperation([CallerMemberName] string memberName = "")
        {
            _name = memberName.ToLower();
        }

        public void Render(DefaultInterpolatedStringHandler stringHandler)
        {
            stringHandler.AppendLiteral(_name);
        }
    }

    public class TableRef : IRenderable
    {
        public void Render(DefaultInterpolatedStringHandler stringHandler)
        {
            throw new NotImplementedException();
        }
    }
}
