using System;

namespace TypedScripts.Exceptions;

public class RequiredParameterUnsetException(string argName, string argType) 
    : Exception($"Argument of type '{argType}' has unset required parameter '{argName}'.");