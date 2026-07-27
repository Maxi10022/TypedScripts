using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
using TypedScripts.Interpreters;
using TypedScripts.Output;

namespace TypedScripts.Executors;

public class BashSshExecutor(SshClient client) : IExecutor
{
    public Interpreter Handles => Interpreter.Bash;
    
    public async Task<IExecutionOutput> ExecuteAsync(ExecutionOptions options, CancellationToken ct = default)
    {

        if (options.Interpreter != Handles)
            throw new NotSupportedException("Executor does not support specified interpreter.");
        
        var formatter = new ArgumentFormatter(options.Interpreter);
        var argLine = options.Arguments.ToArgumentString(formatter);
        var command = $"bash -s -- {argLine}";

        // Immediate guard before actually executing the command.
        ct.ThrowIfCancellationRequested();
        
        var cmd = client.CreateCommand(command);
        var execTask = cmd.ExecuteAsync(ct);
        using var stdin = cmd.CreateInputStream();
        
        var body = Encoding.UTF8.GetBytes(options.Script);
        await stdin.WriteAsync(body, 0, body.Length, ct);
        await stdin.FlushAsync(ct);
        stdin.Close();

        return new SshExecutionOutput(cmd, execTask);
    }
}