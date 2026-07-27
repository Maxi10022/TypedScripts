using NSubstitute;
using TypedScripts.Executors;
using TypedScripts.Interpreters;
using TypedScripts.Output;
using TypedScripts.Tests.Fixtures;

namespace TypedScripts.Tests.Executors;

public abstract class ExecutorTestBase(IntegrationFixture fixture) : IntegrationTestBase(fixture)
{
    protected const string StdOutMarker = "stdout-marker";
    protected const string StdErrMarker = "stderr-marker";
    protected const int LargeOutputByteCount = 2_000_000;
    protected const int PreCancelOutputByteCount = 8192;

    protected abstract Interpreter Interpreter { get; }

    protected abstract IExecutor CreateExecutor();

    protected abstract string ScriptExitsWithCode(int exitCode);

    protected abstract string ScriptWritesToStderrAndStdout();

    protected abstract string ScriptWithInfiniteLoop();

    protected abstract string ScriptWithLargeOutput();

    protected abstract string ScriptWritesThenLoopsForever();

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(42, 42)]
    [InlineData(255, 255)]
    [InlineData(256, 0)]
    [InlineData(257, 1)]
    public async Task Executor_Propagates_Exit_Code_As_Expected(int scriptExitCode, int expectedExitCode)
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptExitsWithCode(scriptExitCode));

        // Act
        var output = await executor.ExecuteAsync(options);
        var exitCode = await output.WaitForExitAsync();

        // Assert
        Assert.Equal(expectedExitCode, exitCode);
    }

    [Fact]
    public async Task Cancellation_Token_Stops_Script_Midst_Execution()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptWithInfiniteLoop());
        var cancellation = new CancellationTokenSource();

        // Act
        var output = await executor.ExecuteAsync(options, cancellation.Token);
        var exception = await Record.ExceptionAsync(async () =>
        {
            var exit = output.WaitForExitAsync();
            await cancellation.CancelAsync();
            await exit;
        });
        
        Assert.IsType<TaskCanceledException>(exception);
    }

    [Fact]
    public async Task Cancel_Stops_Script_Midst_Execution()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptWithInfiniteLoop());
        var output = await executor.ExecuteAsync(options);

        // Act
        var waitForExit = output.WaitForExitAsync();
        output.Cancel();

        // Assert
        var completed = await Task.WhenAny(waitForExit, Task.Delay(TimeSpan.FromSeconds(10)));
        Assert.Same(waitForExit, completed);
        await Assert.ThrowsAsync<TaskCanceledException>(() => waitForExit);
    }

    [Fact]
    public async Task Executor_Does_Not_Throw_For_Non_Zero_Exit_Code()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptExitsWithCode(1));

        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            var output = await executor.ExecuteAsync(options);
            await output.WaitForExitAsync();
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task Executor_Captures_Stdout_And_Stderr_Separately()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptWritesToStderrAndStdout());

        // Act
        var output = await executor.ExecuteAsync(options);
        var (stdOut, stdErr, _) = await ReadToCompletionAsync(output);

        // Assert
        Assert.Contains(StdOutMarker, stdOut);
        Assert.DoesNotContain(StdOutMarker, stdErr);
        Assert.Contains(StdErrMarker, stdErr);
        Assert.DoesNotContain(StdErrMarker, stdOut);
    }

    [Fact]
    public async Task Executor_Throws_When_Interpreter_Does_Not_Match()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptExitsWithCode(0), interpreter: (Interpreter)(-1));

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(() => executor.ExecuteAsync(options));
    }

    [Fact]
    public async Task Executor_Supports_Sequential_Executions()
    {
        // Arrange
        var executor = CreateExecutor();

        // Act
        var firstOutput = await executor.ExecuteAsync(BuildExecutionOptions(ScriptExitsWithCode(1)));
        var firstExitCode = await firstOutput.WaitForExitAsync();
        firstOutput.Dispose();

        var secondOutput = await executor.ExecuteAsync(BuildExecutionOptions(ScriptExitsWithCode(2)));
        var secondExitCode = await secondOutput.WaitForExitAsync();
        secondOutput.Dispose();

        // Assert
        Assert.Equal(1, firstExitCode);
        Assert.Equal(2, secondExitCode);
    }

    [Fact]
    public async Task Executor_Streams_Large_Output_Without_Deadlocking()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptWithLargeOutput());

        // Act
        var output = await executor.ExecuteAsync(options);
        var (stdOut, _, exitCode) = await ReadToCompletionAsync(output).WaitAsync(TimeSpan.FromSeconds(30));

        // Assert
        Assert.Equal(0, exitCode);
        Assert.Equal(LargeOutputByteCount, stdOut.Length);
    }

    [Fact]
    public async Task Executor_Throws_Operation_Canceled_When_Token_Cancelled_Before_Script_Execution()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptExitsWithCode(0));
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            var output = await executor.ExecuteAsync(options, cancellation.Token);
            await output.WaitForExitAsync();
        });
        
        Assert.IsType<OperationCanceledException>(exception);
    }

    [Fact]
    public async Task Dispose_After_Completion_Does_Not_Throw()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptExitsWithCode(0));
        var output = await executor.ExecuteAsync(options);
        await output.WaitForExitAsync();

        // Act
        var exception = Record.Exception(output.Dispose);

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task WaitForExitAsync_Can_Be_Awaited_Multiple_Times()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptExitsWithCode(7));
        var output = await executor.ExecuteAsync(options);

        // Act
        var first = await output.WaitForExitAsync();
        var second = await output.WaitForExitAsync();

        // Assert
        Assert.Equal(7, first);
        Assert.Equal(7, second);
    }

    [Fact]
    public async Task Cancellation_Preserves_Output_Written_Before_Cancel()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptWritesThenLoopsForever());
        using var cancellation = new CancellationTokenSource();
        var output = await executor.ExecuteAsync(options, cancellation.Token);

        // Act
        var marker = new byte[PreCancelOutputByteCount];
        await output.StandardOutput.ReadExactlyAsync(marker);
        await cancellation.CancelAsync();
        
        await Assert.ThrowsAsync<TaskCanceledException>(output.WaitForExitAsync);
        Assert.All(marker, b => Assert.Equal((byte)'x', b));
    }

    [Fact]
    public async Task Dispose_While_Running_Stops_Execution()
    {
        // Arrange
        var executor = CreateExecutor();
        var options = BuildExecutionOptions(ScriptWithInfiniteLoop());
        var output = await executor.ExecuteAsync(options);

        // Act
        output.Dispose();

        // Assert
        var waitForExit = output.WaitForExitAsync();
        var completed = await Task.WhenAny(waitForExit, Task.Delay(TimeSpan.FromSeconds(10)));
        Assert.Same(waitForExit, completed);
    }
    
    private static async Task<(string StandardOutput, string StandardError, int ExitCode)> ReadToCompletionAsync(
        IExecutionOutput output)
    {
        var exitCode = await output.WaitForExitAsync();
        var stdOut = await new StreamReader(output.StandardOutput).ReadToEndAsync();
        var stdErr = await new StreamReader(output.StandardError).ReadToEndAsync();

        return (stdOut, stdErr, exitCode);
    }

    private ExecutionOptions BuildExecutionOptions(string script, Interpreter? interpreter = null) =>
        new(
            script: script,
            interpreter: interpreter ?? Interpreter,
            Substitute.For<IArgumentContainer>()
        );
}
