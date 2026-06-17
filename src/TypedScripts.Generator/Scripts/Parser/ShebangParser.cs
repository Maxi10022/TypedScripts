using System;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Text;
using TypedScripts.Common.Parser;
using static TypedScripts.Scripts.Generation.ScriptDiagnostics;

namespace TypedScripts.Scripts.Parser;

// Parses the interpreter from a shebang line (e.g. '#!/bin/bash' or '#!/usr/bin/env bash')
public class ShebangParser : ILineParser
{
    private static readonly Regex ShebangPattern = new(
        // line start, the shebang marker '#!', optional space
        @"^#!\s*" +

        // the interpreter path, e.g. '/bin/bash' or '/usr/bin/env'
        @"(?<path>\S+)" +

        // an optional argument, used by the 'env' form to name the interpreter (e.g. '/usr/bin/env bash')
        @"(?:\s+(?<arg>\S+))?",
        RegexOptions.Compiled);

    public ILineParseResult Parse(TextLine line)
    {
        if (line.LineNumber != 0) return ParseResults.Skip();
        
        var match = ShebangPattern.Match(line.ToString());
        if (!match.Success) return ParseResults.Skip();

        var path = match.Groups["path"].Value;
        var arg = match.Groups["arg"].Value;

        // the command is the path's final segment, e.g. '/usr/bin/env' -> 'env', '/bin/bash' -> 'bash'
        var command = path.Substring(path.LastIndexOf('/') + 1);

        // the 'env' form defers the real interpreter to the following argument (e.g. '/usr/bin/env bash')
        var value = command.Equals("env", StringComparison.OrdinalIgnoreCase) ? arg : command;

        return Enum.TryParse<Interpreter>(value, true, out var interpreter)
            ? ParseResults.Success(interpreter)
            : ParseResults.Failure(InterpreterNotSupported(value));
    }
}
