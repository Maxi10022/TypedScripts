using System;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using TypedScripts.Interpreters;
using TypedScripts.Output;

namespace TypedScripts.Executors;

public class BashLocalExecutor : IExecutor
{
    private const string Executable = "bash";

    public Interpreter Handles => Interpreter.Bash;

    public Task<IExecutionOutput> ExecuteAsync(ExecutionOptions options, CancellationToken ct = default)
    {
        if (options.Interpreter != Handles)
            throw new NotSupportedException("Executor does not support specified interpreter.");
        
        var formatter = new ArgumentFormatter(options.Interpreter);
        var argLine = options.Arguments.ToArgumentString(formatter);
        var arguments = $"-s -- {argLine}";

        var cancellation = CancellationTokenSource.CreateLinkedTokenSource(ct);

        // Disable pipe blocking
        var pipeOptions = new PipeOptions(pauseWriterThreshold: 0, resumeWriterThreshold: 0);
        var stdout = new Pipe(pipeOptions);
        var stderr = new Pipe(pipeOptions);
        
        var cmd = Cli.Wrap(Executable)
            .WithArguments(arguments)
            .WithStandardInputPipe(PipeSource.FromBytes(Encoding.UTF8.GetBytes(options.Script)))
            .WithStandardOutputPipe(PipeTarget.ToStream(stdout.Writer.AsStream()))
            .WithStandardErrorPipe(PipeTarget.ToStream(stderr.Writer.AsStream()))
            .WithValidation(CommandResultValidation.None);

        // Immediate guard before actually executing the command.
        ct.ThrowIfCancellationRequested();
        
        var execTask = RunAsync(cmd, stdout.Writer, stderr.Writer, cancellation.Token);

        return Task.FromResult<IExecutionOutput>(
            new LocalExecutionOutput(execTask, cancellation, stdout.Reader.AsStream(), stderr.Reader.AsStream()));
    }

    private static async Task<int> RunAsync(Command cmd, PipeWriter stdout, PipeWriter stderr, CancellationToken ct)
    {
        Exception failure = null;
        try
        {
            var result = await cmd.ExecuteAsync(ct).ConfigureAwait(false);
            return result.ExitCode;
        }
        catch (OperationCanceledException e)
        {
            // Normalize CliWrap's cancellation exception to 'TaskCanceledException' to satisfy spec.  
            var canceled = new TaskCanceledException(e.Message, e);
            failure = canceled;
            throw canceled;
        }
        catch (Exception e)
        {
            failure = e;
            throw;
        }
        finally
        {
            // Signal EOF (or surface the failure) so a reader blocked on the pipe unblocks instead of hanging.
            await stdout.CompleteAsync(failure);
            await stderr.CompleteAsync(failure);
        }
    }
}
