using System;

namespace TypedScripts.Exceptions;

public class InterpreterNotSupportedException(Interpreter interpreter, string scriptName) 
    : Exception($"Interpreter '{interpreter.GetName()}' is not supported by {scriptName}");