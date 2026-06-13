using System;

namespace TypedScripts.Arguments.Exceptions;

public class UnsupportedArgumentDefaultException(string type) 
    : Exception($"Type '{type}' does not support default values.");