using TypedScripts.Interpreters;

namespace TypedScripts.Reference;

// DTO for carrying script arguments
public class ExampleScriptArgs : IArgumentContainer
{
    // START: Configured parameter data 
    public string? Database { get; set; }
    public string? OutputDir { get; set; } = "/var/backups";
    public int Port { get; set; } = 5432;
    // END: Configured parameter data
    
    // Default argument string conversion
    public virtual string ToArgumentString(ArgumentFormatter formatter)
    {
        return $"{formatter.Escape(Database)} {formatter.Escape(OutputDir)} db-port={formatter.Escape(Port)}";
    }
}