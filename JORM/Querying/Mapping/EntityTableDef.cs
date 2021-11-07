using System;

namespace JORM.Querying.SqlBuilding;

public class EntityTableDef
{
    protected Func<object> _entityObjCtor;

    public Type EntityType { get; }
    public string TableName { get; }

    public EntityTableDef(Type entityType, string tableName)
    {
        this.EntityType = entityType;
        this.TableName = tableName;
        _entityObjCtor = () => Activator.CreateInstance(entityType)!;
    }

    public EntityTableDef(Type entityType, string tableName, Func<object> entityCtor)
    {
        this.EntityType = entityType;
        this.TableName = tableName;
        _entityObjCtor = entityCtor;
    }

    internal object Construct() => _entityObjCtor();
}