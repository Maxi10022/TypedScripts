using System.IO;
using System.Threading.Tasks;

namespace TypedScripts.Output;

public class LocalExecutionOutput(Task<int> execTask, Stream standardOutput, Stream standardError) : IExecutionOutput
{
    public Stream StandardOutput => standardOutput;
    public Stream StandardError => standardError;

    public async Task<int> WaitForExitAsync() => await execTask.ConfigureAwait(false);

    public void Dispose()
    {
        // Completes the underlying pipe reader
        standardOutput.Dispose(); 
        standardError.Dispose();
    }
}
