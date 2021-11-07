

using System;

namespace JORM
{
    public delegate bool TextPredicate(text text);

    public ref struct TextReader
    {
        public readonly text Text;
        public int Index;
        public int Length => Text.Length;
        
        public text Remaining => Text.Slice(Index);
        public bool CanRead => Index < Length;
        public char Character => Text[Index];

        public char this[int index] => Text[index];
        public text this[Range range] => Text[range];

        public TextReader(text text)
        {
            this.Text = text;
            this.Index = 0;
        }

        public bool MoveNext()
        {
            if (Index < Length)
            {
                Index++;
                return true;
            }
            return false;
        }

        public char Peek()
        {
            var nextIndex = Index + 1;
            if (nextIndex >= Length)
                return default(char);
            return Text[nextIndex];
        }

        public bool TryPeek(out char ch)
        {
            var nextIndex = Index + 1;
            if (nextIndex >= Length)
            {
                ch = default(char);
                return false;
            }
            ch = Text[nextIndex];
            return true;
        }

        public void SkipWhiteSpace()
        {
            while (Index < Length && char.IsWhiteSpace(Text[Index]))
            {
                Index++;
            }
        }

        public text TakeWhiteSpace()
        {
            int start = Index;
            while (Index < Length && char.IsWhiteSpace(Text[Index]))
            {
                Index++;
            }
            return Text.Slice(start, Index - start);
        }

        public text TakeDigits()
        {
            int start = Index;
            while (Index < Length && char.IsDigit(Text[Index]))
            {
                Index++;
            }
            return Text.Slice(start, Index - start);
        }

        public text TakeUntil(TextPredicate examineRemaining)
        {
            int start = Index;
            while (Index < Length)
            {
                if (examineRemaining(Text.Slice(Index)))
                {
                    break;
                }
            }
            return Text.Slice(start, Index - start);
        }

        public text TakeWhile(TextPredicate examineRemaining)
        {
            int start = Index;
            while (Index < Length)
            {
                if (!examineRemaining(Text.Slice(Index)))
                {
                    break;
                }
            }
            return Text.Slice(start, Index - start);
        }

        public text Slice(int start, int length) => Text.Slice(start, length);

    }
}