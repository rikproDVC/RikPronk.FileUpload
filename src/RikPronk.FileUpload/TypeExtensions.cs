using System;

namespace RikPronk.FileUpload.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsArrayOf<T>(this Type type)
        {
            return type == typeof(T[]);
        }
    }
}
