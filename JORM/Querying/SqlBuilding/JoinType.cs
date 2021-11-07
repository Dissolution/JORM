using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JORM.Querying.SqlBuilding
{
    public readonly struct JoinType : IEquatable<JoinType>
    {
        public static bool operator ==(JoinType x, JoinType y) => x._id == y._id;
        public static bool operator !=(JoinType x, JoinType y) => x._id != y._id;

        internal static readonly JoinType Default = new JoinType(0, "?");
        internal static readonly JoinType From = new JoinType(1);
        public static readonly JoinType Inner = new JoinType(2);
        public static readonly JoinType Left = new JoinType(3);
        public static readonly JoinType Right = new JoinType(4);
        public static readonly JoinType Full = new JoinType(5);
        public static readonly JoinType Outer = new JoinType(5);
        public static readonly JoinType Union = new JoinType(6);
        public static readonly JoinType Cross = new JoinType(7);

        public static bool TryParse(ReadOnlySpan<char> text, out JoinType join)
        {
            if (TextHelper.Equals(text, nameof(From), StringComparison.OrdinalIgnoreCase))
            {
                join = From;
                return true;
            }

            if (TextHelper.Equals(text, nameof(Inner), StringComparison.OrdinalIgnoreCase))
            {
                join = Inner;
                return true;
            }

            if (TextHelper.Equals(text, nameof(Left), StringComparison.OrdinalIgnoreCase))
            {
                join = Left;
                return true;
            }

            if (TextHelper.Equals(text, nameof(Right), StringComparison.OrdinalIgnoreCase))
            {
                join = Right;
                return true;
            }
            if (TextHelper.Equals(text, nameof(Full), StringComparison.OrdinalIgnoreCase))
            {
                join = Full;
                return true;
            }

            if (TextHelper.Equals(text, nameof(Outer), StringComparison.OrdinalIgnoreCase))
            {
                join = Outer;
                return true;
            }

            if (TextHelper.Equals(text, nameof(Union), StringComparison.OrdinalIgnoreCase))
            {
                join = Union;
                return true;
            }

            if (TextHelper.Equals(text, nameof(Cross), StringComparison.OrdinalIgnoreCase))
            {
                join = Cross;
                return true;
            }

            join = Default;
            return false;
        }

        private readonly int _id;
        private readonly string _name;

        private JoinType(int id, [CallerMemberName] string memberName = "")
        {
            _id = id;
            _name = memberName;
        }

        public bool Equals(JoinType join) => join._id == _id;

        public override bool Equals(object? obj) => obj is JoinType join && join._id == _id;

        public override int GetHashCode() => _id;

        public override string ToString() => _name;
    }
}