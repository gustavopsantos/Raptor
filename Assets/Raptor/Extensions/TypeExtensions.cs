using System;
using System.Linq;

namespace Raptor.Extensions
{
    public static class TypeExtensions
    {
        public static string ReadableName(this Type type)
        {
            if (type.IsGenericType)
            {
                var genericArguments = string.Join(", ", type.GetGenericArguments().Select(ReadableName));
                return $"{type.Name.Substring(0, type.Name.IndexOf("`"))}<{genericArguments}>";
            }

            return type.Name;
        }
    }
}