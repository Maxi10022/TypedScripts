using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TypedScripts;

// Script -> Interpreter -> Execution Target
// 1. Provide script in temporary file
// 2. Use correct interpreter for script "execution" ("bash ./script.sh")
// 3. Execute script on execution target (Local Bash, Local PowerShell, SSH Bash, WSL)
// 4. Forward stdin, stdout, stderr streams
// 5. Cleanup temporary script

// Once these basics stand implement niceties on top of that - e.g. simple async input->output method calls. 

// TODO come up with the rest of the infrastructure side of shell running things
// This includes some base implementations for CMD, PowerShell, bash, sh, zsh and SSH.
// Maybe reconsider current implementation, a bash script could be executable on SSH just like locally via bash!

public interface IShellExecutor
{
    public Interpreter Handles { get; }
    
    public Task<IShellSession> StartAsync(ShellInvocation invocation, CancellationToken ct = default);
}

// TODO consider moving the shell-dispatcher to code-gen to support native script execution grouping options
// e.g. given an instance of 'ShellDispatcher': shellDispatcher.Maintenance.BackupDatabaseAsync(...);
// In the sample above the "Maintenance" property would be a type just for grouping script executions behind some facade.

/// <summary>
/// Default entrypoint for executing scripts with TypedScripts.
/// </summary>
public class ShellDispatcher
{
    private readonly IReadOnlyDictionary<Interpreter, IShellExecutor> _executors;

    // Inject all available shell executors, currently does not handle duplicate shell handlers.
    public ShellDispatcher(IEnumerable<IShellExecutor> executors)
    {
        _executors = executors.ToDictionary(e => e.Handles);
    }

    // If moved to codegen this method can be made internal, optional.
    public Task<IShellSession> StartAsync(ShellInvocation invocation, CancellationToken ct = default)
    {
        return _executors.TryGetValue(invocation.Interpreter, out IShellExecutor executor)
            ? executor.StartAsync(invocation, ct)
            : throw new NotSupportedException(
                $"Script execution with shell '{invocation.Interpreter.GetShellName()}' is not yet supported.");
    }
}

public interface IShellSession : IDisposable
{
    public Stream StandardInput { get; }
    
    public Stream StandardOutput { get; }
    
    public Stream StandardError { get; }
    
    public Task<int> WaitForExitAsync(CancellationToken ct = default); 
}

/// <summary>
/// Typically never user constructed, usually constructed by the code-generated script object. 
/// </summary>
/// <param name="script">The actual script content to execute.</param>
/// <param name="interpreter"></param>
/// <param name="arguments"></param>
public class ShellInvocation(string script, Interpreter interpreter, IReadOnlyList<string> arguments)
{
    /// <summary>
    /// The actual script to execute.
    /// </summary>
    public string Script { get; } = script;
    
    /// <summary>
    /// The shell to use to execute that script.
    /// </summary>
    public Interpreter Interpreter { get; } = interpreter;
    
    /// <summary>
    /// Ordered list of script arguments, order must be preserved.
    /// This means the first item must be the first argument etc.
    /// </summary>
    public IReadOnlyList<string> Arguments { get; } = arguments;
}