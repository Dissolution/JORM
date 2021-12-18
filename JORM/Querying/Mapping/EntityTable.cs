// using System;
//
// namespace JORM.Querying.SqlBuilding
// {
//     public class EntityTable : Table
//     {
//         public Type EntityType { get; }
//
//         public EntityTable(Type entityType, string? alias = null)
//             : base(entityType.Name, alias)
//         {
//             this.EntityType = entityType;
//         }
//
//         public override string ToString()
//         {
//             if (string.IsNullOrWhiteSpace(Alias))
//                 return $"[{EntityType.FullName}]";
//             return $"[{EntityType.FullName}] {Alias}";
//         }
//     }
// }