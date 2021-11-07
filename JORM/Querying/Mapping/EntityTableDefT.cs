using System;

namespace JORM.Querying.SqlBuilding
{
    public class EntityTableDef<TEntity> : EntityTableDef
        where TEntity : class
    {
        public EntityTableDef(string tableName)
            : base(typeof(TEntity), tableName, Activator.CreateInstance<TEntity>)
        {
            
        }

        internal new TEntity Construct() => (_entityObjCtor() as TEntity)!;
    }
}