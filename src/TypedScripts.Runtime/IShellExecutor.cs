using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TypedScripts.Runtime;

// TODO come up with the rest of the infrastructure side of shell running things
// This includes some base implementations for CMD, PowerShell, bash, sh, zsh and SSH.
// Maybe reconsider current implementation, a bash script could be executable on SSH just like locally via bash!

public interface IShellExecutor
{
    public Shell Handles { get; }
    
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
    private readonly IReadOnlyDictionary<Shell, IShellExecutor> _executors;

    // Inject all available shell executors, currently does not handle duplicate shell handlers.
    public ShellDispatcher(IEnumerable<IShellExecutor> executors)
    {
        _executors = executors.ToDictionary(e => e.Handles);
    }

    // If moved to codegen this method can be made internal, optional.
    public Task<IShellSession> StartAsync(ShellInvocation invocation, CancellationToken ct = default)
    {
        return _executors.TryGetValue(invocation.Shell, out IShellExecutor executor)
            ? executor.StartAsync(invocation, ct)
            : throw new NotSupportedException(
                $"Script execution with shell '{invocation.Shell.GetShellName()}' is not yet supported.");
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
/// <param name="shell"></param>
/// <param name="arguments"></param>
public class ShellInvocation(string script, Shell shell, IReadOnlyList<string> arguments)
{
    /// <summary>
    /// The actual script to execute.
    /// </summary>
    public string Script { get; } = script;
    
    /// <summary>
    /// The shell to use to execute that script.
    /// </summary>
    public Shell Shell { get; } = shell;
    
    /// <summary>
    /// Ordered list of script arguments, order must be preserved.
    /// This means the first item must be the first argument etc.
    /// </summary>
    public IReadOnlyList<string> Arguments { get; } = arguments;
}