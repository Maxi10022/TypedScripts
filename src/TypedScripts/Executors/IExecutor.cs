using System;
using System.Threading;
using System.Threading.Tasks;
using TypedScripts.Output;

namespace TypedScripts.Executors;

/// <summary>
/// Interface used to implement interpreter executions.
/// A single interpreter might have more than one implementation,
/// if they target different execution targets (e.g. SSH vs Local).
/// </summary>
public interface IExecutor
{
    /// <summary>
    /// The <see cref="Interpreter"/> this executor supports, used for validation in <see cref="ExecuteAsync"/>. 
    /// </summary>
    public Interpreter Handles { get; }
    
    /// <summary>
    /// Executes the script and returns a streamed response.
    /// </summary>
    /// <param name="options">Options carrying script data.</param>
    /// <param name="ct">Cancellation token used to cancel the async <see cref="IExecutionOutput"/>.</param>
    /// <returns>Instance of <see cref="IExecutionOutput"/>.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation was requested before execution.</exception>
    /// <exception cref="TaskCanceledException">Thrown when an already running execution was canceled.</exception>
    public Task<IExecutionOutput> ExecuteAsync(ExecutionOptions options, CancellationToken ct = default);
}