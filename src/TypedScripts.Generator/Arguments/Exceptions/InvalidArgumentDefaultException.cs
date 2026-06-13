using System;

namespace TypedScripts.Arguments.Exceptions;

public class InvalidArgumentDefaultException(string value, string type) 
    : Exception($"Default value '{value}' is invalid for type '{type}'.");