using System;
using System.Runtime.CompilerServices;

namespace JORM
{
    public readonly record struct DbParam(string Name, Type Type, object? Value);

    // This attribute let's C# know we're making an interpolated string handler
    [InterpolatedStringHandler]
    // The handler should usually be a "ref struct", meaning it only lives on the stack.
    // This may be a limitation if you want to allow "await" within the holes in the expression, so "ref" may be removed in that case.
    // However, this example requires "ref struct" because it includes a DefaultInterpolatedStringHandler in its fields.
    public ref struct InterpolatedSqlHandler
    {
        // Internally we'll use DefaultInterpolatedStringHandler to build the query string.
        // This be more performant than reinventing the wheel.
        private DefaultInterpolatedStringHandler _stringHandler;

        // This will maintain a list of parameters as we build the query string
        public readonly DbParam[] Parameters;

        // The number of parameters added so far
        private int _parameterCount;

        public InterpolatedSqlHandler(int literalLength, int formattedCount)
        {
            // Construct the inner handler, forwarding the same hints
            _stringHandler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);

            // Build an empty list of parameters with the capacity we'll need
            Parameters = new DbParam[formattedCount];
            _parameterCount = 0;
        }

        public void AppendLiteral(string value)
        {
            // In this example, literals represent query text like "SELECT ..."
            // Forward literals to the inner handler to be added to the query string
            _stringHandler.AppendLiteral(value);
        }

        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            // Other backing implementations may be able to optimize this to avoid allocating a string
            // SqlParameters need strings not char spans, so forward to that implementation
            AppendFormatted(value.ToString(), null);
        }

        public void AppendFormatted<T>(T value) => AppendFormatted<T>(value, null);

        // There are a lot of AppendFormatted overloads we're required to implement
        // We could use alignment and format parameters for our own purposes, here we ignore them

        public void AppendFormatted<T>(T value, string? format)
        {
            string name = GetParameterName(format);
            _stringHandler.AppendLiteral(name);
            Parameters[_parameterCount++] = new DbParam(name, typeof(T), value);
        }

        public void AppendFormatted<T>(T value, int alignment, string? format) => AppendFormatted(value, format);

        public void AppendFormatted<T>(T value, int alignment) => AppendFormatted(value, null);

        // public void AppendFormatted(string? value, [CallerArgumentExpression("value")] string name = "") =>
        //     AppendParameter(SqlDbType.NVarChar, value);
        //
        // public void AppendFormatted(string? value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string name = "") =>
        //     AppendParameter(SqlDbType.NVarChar, value);

        private string GetParameterName(string? name)
        {
            if (name is null)
            {
                return $"@p{_parameterCount}";
            }

            var len = name.Length;
            if (len == 0)
            {
                return $"@p{_parameterCount}";
            }

            Span<char> paramNameBuffer = stackalloc char[name.Length + 2];

            // Assume that a valid parameter name must only be 0-9a-zA-Z            TODO: RESEARCH
            paramNameBuffer[0] = '@';
            int n = 1;
            char ch;
            for (var i = 0; i < len; i++)
            {
                ch = name[i];
                if (char.IsLetter(ch))
                {
                    paramNameBuffer[n++] = ch;
                }
                else if (char.IsDigit(ch))
                {
                    if (n == 1)
                    {
                        paramNameBuffer[n++] = 'p';
                    }

                    paramNameBuffer[n++] = ch;
                }
            }

            // Did we get no valid characters?
            if (n == 1)
            {
                paramNameBuffer[1] = 'p';
                if (_parameterCount.TryFormat(paramNameBuffer.Slice(2), out int charsWritten))
                {
                    return new string(paramNameBuffer.Slice(0, n + charsWritten));
                }

                paramNameBuffer.Clear();
                return string.Create(null, paramNameBuffer, $"@p{_parameterCount}");
            }

            return new string(paramNameBuffer.Slice(0, n));
        }

        // Forward to the inner handler
        public readonly override string ToString() => _stringHandler.ToString();

        // Forward to the inner handler
        public string ToStringAndClear() => _stringHandler.ToStringAndClear();
    }
}
