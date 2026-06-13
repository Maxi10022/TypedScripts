using System;
using Microsoft.CodeAnalysis;

namespace TypedScripts.Scripts.Exceptions;

public class InvalidScriptSyntaxException(Diagnostic[] diagnostics) : Exception
{
    public Diagnostic[] Diagnostics { get; } = diagnostics;
}