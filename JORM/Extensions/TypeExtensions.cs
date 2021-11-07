using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace JORM.Extensions
{
    public static class TypeExtensions
    {
        public static bool CanBeNull(this Type? type)
        {
            return type is null || type.IsClass || type.IsAbstract || type.IsInterface ||
                   typeof(Nullable<>).IsAssignableFrom(type);
        }
    }
}
