using TypedScripts.Tests.Fixtures;

namespace TypedScripts.Tests.Executors.Bash;

[Collection(nameof(IntegrationCollection))]
public abstract class BashExecutorTestBase(IntegrationFixture fixture) : ExecutorTestBase(fixture)
{
    protected override Interpreter Interpreter => Interpreter.Bash;

    protected override string ScriptExitsWithCode(int exitCode) =>
        $"""
         #!/bin/bash
         exit {exitCode}
         """;

    protected override string ScriptWritesToStderrAndStdout() =>
        $"""
         #!/bin/bash
         echo "{StdOutMarker}"
         echo "{StdErrMarker}" >&2
         """;

    protected override string ScriptWithInfiniteLoop() =>
        """
        #!/bin/bash
        while :; do sleep 1; done
        """;

    protected override string ScriptWithLargeOutput() =>
        $"""
         #!/bin/bash
         yes "0123456789" | head -c {LargeOutputByteCount}
         """;

    protected override string ScriptWritesThenLoopsForever() =>
        $"""
         #!/bin/bash
         head -c {PreCancelOutputByteCount} /dev/zero | tr '\0' 'x'
         while :; do sleep 1; done
         """;
}
