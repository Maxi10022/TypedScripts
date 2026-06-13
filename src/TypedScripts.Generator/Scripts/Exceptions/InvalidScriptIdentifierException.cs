using System;

namespace TypedScripts.Scripts.Exceptions;

public class InvalidScriptIdentifierException(string name) 
    : Exception($"Script name '{name}' is not a valid C# identifier.");