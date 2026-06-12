using System;

namespace TypedScripts.Arguments.Exceptions;

public class InvalidParameterIdentifierException(string name) 
    : Exception($"Parameter name '{name}' is not a valid C# identifier.");