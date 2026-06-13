using System;

namespace TypedScripts.Arguments.Exceptions;

public class UnsupportedArgumentTypeException(string type) 
    : Exception($"Type '{type}' is not a supported script argument type.");