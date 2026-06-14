using Microsoft.CodeAnalysis;

namespace TypedScripts.Common.Parser;

public static class ParseResults
{
    public static SkipLineParseResult Skip() => SkipLineParseResult.Instance;
    
    public static LineParseFailure Failure(params DiagnosticDescriptor[] descriptors) =>  new(descriptors);
    
    public static LineParseSuccess<T> Success<T>(T value) => new(value);
}