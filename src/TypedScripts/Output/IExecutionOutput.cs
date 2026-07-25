using System;
using System.IO;
using System.Threading.Tasks;

namespace TypedScripts.Output;

/// <summary>
/// Object containing streams callers can read/pull data from.
/// </summary>
public interface IExecutionOutput : IDisposable
{
    /// <summary>
    /// The standard-out stream.
    /// </summary>
    public Stream StandardOutput { get; }
    
    /// <summary>
    /// The standard-error stream.
    /// </summary>
    public Stream StandardError { get; }
    
    /// <summary>
    /// Wait for the scripts execution to complete.
    /// <b>Cancellation token passed when <c>Execute</c> was called cancels the task!</b> 
    /// </summary>
    /// <returns>The scripts exit code.</returns>
    public Task<int> WaitForExitAsync(); 
}