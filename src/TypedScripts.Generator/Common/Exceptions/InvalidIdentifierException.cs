using System;

namespace TypedScripts.Common.Exceptions;

public class InvalidIdentifierException(string identifier) 
    : Exception($"The string '{identifier}' is not a valid C# identifier.");