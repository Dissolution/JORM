global using text = System.ReadOnlySpan<char>;

using System;

namespace JORM.Querying.SqlBuilding
{
    public static class TextHelper
    {
       

        public static void SkipWhiteSpace(this ReadOnlySpan<char> text, ref int index)
        {
            if (index >= 0 && index < text.Length)
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
            if (index >= 0 && index < text.Length)
            {
                int start = index;
                while (char.IsDigit(text[index]))
                {
                    index++;
                    if (index == text.Length)
                        break;
                }
                return text.Slice(start, index - start);
            }
            else
            {
                return default;
            }
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

        public static bool Equals(string? a, string? b, StringComparison comparison = default)
        {
            return MemoryExtensions.Equals(a, b, comparison);
        }

        public static bool Equals(string? a, text b, StringComparison comparison = default)
        {
            return MemoryExtensions.Equals(a, b, comparison);
        }

        public static bool Equals(text a, string? b, StringComparison comparison = default)
        {
            return MemoryExtensions.Equals(a, b, comparison);
        }

        public static bool Equals(text a, text b, StringComparison comparison = default)
        {
            return MemoryExtensions.Equals(a, b, comparison);
        }
    }
}