using System.Linq;
using StringComparer = System.StringComparer;

namespace TypedScripts.Arguments;

public class SupportedArgumentTypes
{
    public static readonly string[] All = 
    [
        "string",
        "bool",
        "sbyte",
        "char",
        "decimal",
        "double",
        "float",
        "int",
        "uint",
        "long",
        "ulong",
        "short",
        "ushort"
    ];
    
    public static bool IsSupportedArgumentType(string type) =>
        All.Contains(type, StringComparer.InvariantCultureIgnoreCase);
}