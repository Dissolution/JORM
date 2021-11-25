using System;

namespace JORM.Querying.Builder;

public sealed class SQL : IEquatable<SQL>
{
    public static implicit operator string(SQL sql) => sql.ToString();

    public string Format { get; }
    public object?[] Arguments { get; }

    /* TODO:
     * We really want Render() support as well as some parse(string,params) + parse(formattablestring) methods that let us find holes and pre-mark them,
     * break them into string[] chunks, whatever we need them to be
     *
     */

    public SQL(RawString format, params object?[] arguments)
    {
        this.Format = (string)format;
        this.Arguments = arguments;
    }

    public SQL(FormattableString format)
    {
        this.Format = format.Format;
        this.Arguments = format.GetArguments();
    }

    public bool Equals(SQL? sql)
    {
        return sql is not null &&
               string.Equals(sql.Format, Format, StringComparison.OrdinalIgnoreCase) &&
               ArgsEqualityComparer.Instance.Equals(sql.Arguments, Arguments);
    }

    public override bool Equals(object? obj)
    {
        if (obj is SQL sql)
            return Equals(sql);
        // TODO: Parse string/Formattablestring/etc
        return false;
    }

    public override int GetHashCode()
    {
        var hasher = new HashCode();
        hasher.Add(Format);
        hasher.Add(Arguments, ArgsEqualityComparer.Instance);
        return hasher.ToHashCode();
    }

    public override string ToString()
    {
        return string.Format(Format, Arguments);
    }

   
}