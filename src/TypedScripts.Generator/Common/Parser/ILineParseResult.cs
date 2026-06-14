using Microsoft.CodeAnalysis;

namespace TypedScripts.Common.Parser;

public interface ILineParseResult;

public class SkipLineParseResult : ILineParseResult
{
    public static readonly SkipLineParseResult Instance = new();
    private SkipLineParseResult() {}
}

public class LineParseSuccess<T>(T value) : ILineParseResult
{
    public T Value { get; } = value;
}

public class LineParseFailure(params DiagnosticDescriptor[] descriptors) : ILineParseResult
{
    public DiagnosticDescriptor[] Descriptors { get; } = descriptors;
}