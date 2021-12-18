using System;
using System.Collections.Generic;

namespace JORM
{
    public abstract class DbTypeMap
    {
        public static DbTypeMap Default { get; } = new DefaultDbTypeMap();

        protected readonly Dictionary<Type, DbType> _map;

        public DbType this[Type type] => _map.TryGetValue(type, out var dbType) ? dbType : DbType.Object;

        protected DbTypeMap()
        {
            _map = new Dictionary<Type, DbType>(64);
        }

        public bool TryGetDbType(Type type, out DbType dbType)
        {
            return _map.TryGetValue(type, out dbType);
        }

        public bool TryGetDbType<T>(out DbType dbType)
        {
            return _map.TryGetValue(typeof(T), out dbType);
        }
    }

    public abstract class DbTypeMap<TDbType>
        where TDbType : struct, Enum
    {
        protected readonly Dictionary<Type, TDbType> _map;

        public TDbType this[Type type] => _map[type];

        protected DbTypeMap()
        {
            _map = new Dictionary<Type, TDbType>(64);
        }

        public bool TryGetDbType(Type type, out TDbType dbType)
        {
            return _map.TryGetValue(type, out dbType);
        }

        public bool TryGetDbType<T>(out TDbType dbType)
        {
            return _map.TryGetValue(typeof(T), out dbType);
        }
    }

    internal sealed class DefaultDbTypeMap : DbTypeMap
    {
        public DefaultDbTypeMap()
        {
            _map.Add(typeof(byte), DbType.Byte);
            _map.Add(typeof(sbyte), DbType.SByte);
            _map.Add(typeof(short), DbType.Int16);
            _map.Add(typeof(ushort), DbType.UInt16);
            _map.Add(typeof(int), DbType.Int32);
            _map.Add(typeof(uint), DbType.UInt32);
            _map.Add(typeof(long), DbType.Int64);
            _map.Add(typeof(ulong), DbType.UInt64);
            _map.Add(typeof(float), DbType.Single);
            _map.Add(typeof(double), DbType.Double);
            _map.Add(typeof(decimal), DbType.Decimal);
            _map.Add(typeof(bool), DbType.Boolean);
            _map.Add(typeof(string), DbType.String);
            _map.Add(typeof(char), DbType.StringFixedLength);
            _map.Add(typeof(Guid), DbType.Guid);
            _map.Add(typeof(DateTime), DbType.DateTime);
            _map.Add(typeof(DateTimeOffset), DbType.DateTimeOffset);
            _map.Add(typeof(byte[]), DbType.Binary);
            _map.Add(typeof(byte?), DbType.Byte);
            _map.Add(typeof(sbyte?), DbType.SByte);
            _map.Add(typeof(short?), DbType.Int16);
            _map.Add(typeof(ushort?), DbType.UInt16);
            _map.Add(typeof(int?), DbType.Int32);
            _map.Add(typeof(uint?), DbType.UInt32);
            _map.Add(typeof(long?), DbType.Int64);
            _map.Add(typeof(ulong?), DbType.UInt64);
            _map.Add(typeof(float?), DbType.Single);
            _map.Add(typeof(double?), DbType.Double);
            _map.Add(typeof(decimal?), DbType.Decimal);
            _map.Add(typeof(bool?), DbType.Boolean);
            _map.Add(typeof(char?), DbType.StringFixedLength);
            _map.Add(typeof(Guid?), DbType.Guid);
            _map.Add(typeof(DateTime?), DbType.DateTime);
            _map.Add(typeof(DateTimeOffset?), DbType.DateTimeOffset);
        }
    }
}
