using Microsoft.CodeAnalysis.Text;

namespace TypedScripts.Common.Parser;

public interface ILineParser
{
    public ILineParseResult Parse(TextLine line);
}