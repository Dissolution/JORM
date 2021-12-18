using System;
using System.Data;
using System.Reflection;
using JORM.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace JORM.Querying.SqlBuilding
{
    public class EntityColumnDef
    {
        protected Action<object, object?> _setValue;
        protected Func<object, object?> _getValue;

        public EntityTableDef TableDef { get; }
        public string ColumnName { get; internal set; }
        public Type EntityColumnType { get; internal set; }
        public SqlDbType DbType { get; internal set; }
        public bool Nullable { get; internal set; }

        public EntityColumnDef(EntityTableDef entityTableDef, PropertyInfo entityProperty)
        {
            this.TableDef = entityTableDef;
            this.ColumnName = entityProperty.Name;
            this.EntityColumnType = entityProperty.PropertyType;
            this.DbType = default;
            this.Nullable = EntityColumnType.CanBeNull();
        }

        internal object? GetValue(object entity) => _getValue(entity);
        internal void SetValue(object entity, object? value) => _setValue(entity, value);
    }

    public class EntityColumnDef<TEntity, TColumn> : EntityColumnDef where TEntity : class
    {
        protected Action<TEntity, TColumn> _setValue;
        protected Func<TEntity, TColumn> _getValue;

        public EntityColumnDef(EntityTableDef<TEntity> tableDef, PropertyInfo entityProperty)
            : base(tableDef, entityProperty)
        {

        }
    }

    public interface IDbSource
    {
        SqlDbType GetDbType(PropertyInfo entityProperty);
    }
}