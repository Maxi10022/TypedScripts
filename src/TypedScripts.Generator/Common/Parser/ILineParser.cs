namespace TypedScripts.Common.Parser;

public interface ILineParser
{
    public ILineParseResult Parse(string line);
}