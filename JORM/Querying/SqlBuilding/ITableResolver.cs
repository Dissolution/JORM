using System;

namespace JORM.Querying.SqlBuilding
{
    public interface ITableResolver
    {
        Table GetTable(string name, string? alias = null);
        Table GetTable<TEntity>();
        Table GetTable(Type entityType);
    }
}