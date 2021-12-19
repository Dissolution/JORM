using System;
using System.Runtime.CompilerServices;

namespace JORM.Text;

public delegate bool ScanPredicate(ref int index, ReadOnlySpan<char> remaining);

public ref struct TextReader
{
    private ReadOnlySpan<char> _chars;
    private int _index;

    public ReadOnlySpan<char> Remaining
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_index >= _chars.Length)
                return default;
            return _chars.Slice(_index);
        }
    }

    public TextReader(ReadOnlySpan<char> chars)
    {
        _chars = chars;
        _index = 0;
    }

    public ReadOnlySpan<char> TakeUntil(ScanPredicate scanPredicate)
    {
        int i = _index;
        int start = i;
        while (i < _chars.Length && !scanPredicate(ref i, _chars.Slice(i)))
        {
            //i++;
        }
        _index = i;
        return _chars.Slice(start, i - start);
    }

}