using System;

namespace JORM.Querying.SqlBuilding
{
    public interface IEntityMap : IEquatable<IEntityMap>
    {
        Type EntityType { get; }

    }
}