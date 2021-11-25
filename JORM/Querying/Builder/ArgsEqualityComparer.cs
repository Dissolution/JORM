using System;
using System.Collections.Generic;

namespace JORM.Querying.Builder;

internal sealed class ArgsEqualityComparer : EqualityComparer<object?[]>
{
    public static ArgsEqualityComparer Instance = new ArgsEqualityComparer();

    public override bool Equals(object?[]? x, object?[]? y)
    {
        if (object.Equals(x, y)) return true;
        if (x is null || y is null) return false;
        if (x.Length != y.Length) return false;
        for (int i = 0; i < x.Length; i++)
        {
            if (!object.Equals(x[i], y[i])) return false;
        }
        return true;
    }

    public override int GetHashCode(object?[]? args)
    {
        if (args is null) return 0;
        var hasher = new HashCode();
        for (var i = 0; i < args.Length; i++)
        {
            hasher.Add(args[i]);
        }
        return hasher.ToHashCode();
    }
}