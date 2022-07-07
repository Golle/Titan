using System;
using System.Linq;

namespace Titan.Core;

public static class TypeExtensions
{
    // NOTE(Jens): this allocates memory (select and string format). Decide if we should use it in release/shipping as well.
#if DEBUG
    public static string FormattedName(this Type type)
        => !type.IsGenericType
            ? type.Name
            : $"{type.Name[..type.Name.IndexOf('`')]}<{string.Join(", ", type.GenericTypeArguments.Select(t => t.FormattedName()))}>";
#else
    public static string FormattedName(this Type type) => type.Name;
#endif
}
