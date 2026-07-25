using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;

namespace TypedScripts.Output;

public class SshExecutionOutput(SshCommand cmd, Task execTask) : IExecutionOutput
{
    public void Dispose() => cmd.Dispose();

    public Stream StandardOutput => cmd.OutputStream;
    public Stream StandardError => cmd.ExtendedOutputStream;
    
    public async Task<int> WaitForExitAsync()
    {
        var result = cmd.BeginExecute();
        await execTask.ConfigureAwait(false);
        return cmd.ExitStatus ?? -1;
    }
}