using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace JORM.Text
{
 public ref struct TextWriter
    {
        /// <summary>Minimum size array to rent from the pool.</summary>
        /// <remarks>Same as stack-allocation size used today by string.Format.</remarks>
        private const int MinimumArrayPoolLength = 256;
        
        /// <summary>Array rented from the array pool and used to back <see cref="_chars"/>.</summary>
        private char[]? _arrayToReturnToPool;
        /// <summary>The span to write into.</summary>
        private Span<char> _chars;
        /// <summary>Position at which to write the next character.</summary>
        private int _pos;
        
        /// <summary>Gets a span of the written characters thus far.</summary>
        internal Span<char> Written
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _chars.Slice(0, _pos);
        }

        internal Span<char> Available
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _chars.Slice(_pos);
        }

        public TextWriter(int capacity)
        {
            _chars = _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(Math.Max(MinimumArrayPoolLength, capacity));
            _pos = 0;
        }

        public TextWriter(Span<char> initialBuffer)
        {
            _chars = initialBuffer;
            _arrayToReturnToPool = null;
            _pos = 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GrowThenCopyString(string value)
        {
            Grow(value.Length);
            value.CopyTo(Available);
            _pos += value.Length;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GrowThenCopySpan(ReadOnlySpan<char> value)
        {
            Grow(value.Length);
            value.CopyTo(Available);
            _pos += value.Length;
        }

        /// <summary>Grows <see cref="_chars"/> to have the capacity to store at least <paramref name="additionalChars"/> beyond <see cref="_pos"/>.</summary>
        [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
        private void Grow(int additionalChars)
        {
            // This method is called when the remaining space (_chars.Length - _pos) is
            // insufficient to store a specific number of additional characters.  Thus, we
            // need to grow to at least that new total. GrowCore will handle growing by more
            // than that if possible.
            Debug.Assert(additionalChars > _chars.Length - _pos);
            GrowCore((uint)_pos + (uint)additionalChars);
        }

        /// <summary>Grow the size of <see cref="_chars"/> to at least the specified <paramref name="requiredMinCapacity"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // but reuse this grow logic directly in both of the above grow routines
        private void GrowCore(uint requiredMinCapacity)
        {
            // We want the max of how much space we actually required and doubling our capacity (without going beyond the max allowed length). We
            // also want to avoid asking for small arrays, to reduce the number of times we need to grow, and since we're working with unsigned
            // ints that could technically overflow if someone tried to, for example, append a huge string to a huge string, we also clamp to int.MaxValue.
            // Even if the array creation fails in such a case, we may later fail in ToStringAndClear.

            uint newCapacity = Math.Max(requiredMinCapacity, Math.Min((uint)_chars.Length * 2, 0x3FFFFFDF));
            int arraySize = (int)Math.Clamp(newCapacity, MinimumArrayPoolLength, int.MaxValue);

            char[] newArray = ArrayPool<char>.Shared.Rent(arraySize);
            Written.CopyTo(newArray);

            char[]? toReturn = _arrayToReturnToPool;
            _chars = _arrayToReturnToPool = newArray;

            if (toReturn is not null)
            {
                ArrayPool<char>.Shared.Return(toReturn);
            }
        }

        /// <summary>Ensures <see cref="_chars"/> has the capacity to store <paramref name="additionalChars"/> beyond <see cref="_pos"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAdding(int additionalChars)
        {
            if (_chars.Length - _pos < additionalChars)
            {
                Grow(additionalChars);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(ReadOnlySpan<char> text)
        {
            if (text.TryCopyTo(Available))
            {
                _pos += text.Length;
            }
            else
            {
                GrowThenCopySpan(text);
            }
        }

        /// <summary>Writes the specified string to the handler.</summary>
        /// <param name="text">The string to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(string text)
        {
            if (text.TryCopyTo(Available))
            {
                _pos += text.Length;
            }
            else
            {
                GrowThenCopyString(text);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append<T>(T value)
        {
            string? text;
            if (value is IFormattable)
            {
                // If the value can format itself directly into our buffer, do so.
                if (value is ISpanFormattable)
                {
                    int charsWritten;
                    while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default)) // constrained call avoiding boxing for value types
                    {
                        Grow(16);
                    }
                    _pos += charsWritten;
                    return;
                }

                text = ((IFormattable)value).ToString(default, default); // constrained call avoiding boxing for value types
            }
            else
            {
                text = value?.ToString();
            }

            if (text is not null)
            {
                if (text.TryCopyTo(Available))
                {
                    _pos += text.Length;
                }
                else
                {
                    GrowThenCopyString(text);
                }
            }
        }

        /// <summary>Gets the built <see cref="string"/> and clears the handler.</summary>
        /// <returns>The built string.</returns>
        /// <remarks>
        /// This releases any resources used by the handler. The method should be invoked only
        /// once and as the last thing performed on the handler. Subsequent use is erroneous, ill-defined,
        /// and may destabilize the process, as may using any other copies of the handler after ToStringAndClear
        /// is called on any one of them.
        /// </remarks>
        public string ToStringAndClear()
        {
            string result = new string(Written);
            Dispose();
            return result;
        }

        /// <summary>Clears the handler, returning any rented array to the pool.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // used only on a few hot paths
        public void Dispose()
        {
            char[]? toReturn = _arrayToReturnToPool;
            this = default; // defensive clear
            if (toReturn is not null)
            {
                ArrayPool<char>.Shared.Return(toReturn);
            }
        }

        /// <summary>Gets the built <see cref="string"/>.</summary>
        /// <returns>The built string.</returns>
        public override string ToString() => new string(Written);
    }
}