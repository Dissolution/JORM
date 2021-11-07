/*
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JORM.Text
{
    public sealed class TextBuilder
    {
        private static readonly ArrayPool<char> _charArrayPool = ArrayPool<char>.Shared;

        private char[] _characters;
        private int _length;

        public TextBuilder()
        {
            _characters = _charArrayPool.Rent(1024);
            _length = 0;
        }

        public void Write(text text)
        {
            
        }


        public void Dispose()
        {

        }
    }
}
*/
