using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;

namespace TypedScripts.Output;

public class SshExecutionOutput(SshCommand cmd, Task execTask) : IExecutionOutput
{
    public void Dispose()
    {
        Cancel();
        cmd.Dispose();
    }

    public Stream StandardOutput => cmd.OutputStream;
    public Stream StandardError => cmd.ExtendedOutputStream;
    
    public async Task<int> WaitForExitAsync()
    {
        await execTask.ConfigureAwait(false);
        return cmd.ExitStatus ?? -1;
    }

    public void Cancel() => cmd.CancelAsync(forceKill: true);
}