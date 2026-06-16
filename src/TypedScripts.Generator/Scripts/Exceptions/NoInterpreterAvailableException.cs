using System;

namespace TypedScripts.Scripts.Exceptions;

public class NoInterpreterAvailableException() : Exception("No interpreter available for this script.");