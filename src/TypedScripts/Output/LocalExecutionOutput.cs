using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TypedScripts.Output;

public class LocalExecutionOutput(
    Task<int> execTask, 
    CancellationTokenSource cancellation, 
    Stream standardOutput, 
    Stream standardError) : IExecutionOutput
{
    public Stream StandardOutput => standardOutput;
    public Stream StandardError => standardError;

    public async Task<int> WaitForExitAsync() => await execTask.ConfigureAwait(false);

    public void Dispose()
    {
        Cancel();
        standardOutput.Dispose();
        standardError.Dispose();
        cancellation.Dispose();
    }
    
    public void Cancel() => cancellation.Cancel();
}
