using System.Collections;
using System.Collections.Generic;

namespace TypedScripts.Tests.Arguments;

public class SupportedArgumentTypes : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (var type in TypedScripts.Arguments.SupportedArgumentTypes.All)
        {
            yield return [type];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}