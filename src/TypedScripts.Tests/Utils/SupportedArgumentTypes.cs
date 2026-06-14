using System.Collections;
using System.Collections.Generic;
using TypedScripts.Arguments.ValueTypes;

namespace TypedScripts.Tests.Utils;

public class SupportedArgumentTypes : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (var type in SupportedArgumentType.All)
        {
            yield return [type];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}