using System;

namespace TypedScripts.Reference;

// This implementation is just a reference for how the source generated code will work.
public class ExampleScript
{
    private const string ScriptContent = "#!/bin/bash\n\n# @identifier ExampleScript\n# @param database:string required\n# @param outputDir:string optional default=\"/var/backups\"\n# @param port:int optional default=5432\n# @param compress:bool optional default=true\n# @param retentionDays:int optional default=7\n\nset -euo pipefail\n\n# Positional arguments, read in declaration order. A value is always present at\n# each index because C# passes the @param default (or \"\") for optional args.\ndatabase=\"$1\"\noutput_dir=\"$2\"\nport=\"$3\"\ncompress=\"$4\"\nretention_days=\"$5\"\n\ntimestamp=\"$(date +%Y%m%d_%H%M%S)\"\nbackup_file=\"${output_dir}/${database}_${timestamp}.sql\"\n\necho \"Backing up database '${database}' (port ${port})\"\necho \"Target: ${backup_file}\"\n\nif [ \"$compress\" = \"true\" ]; then\n  backup_file=\"${backup_file}.gz\"\n  echo \"Compression enabled -> ${backup_file}\"\nfi\n\necho \"Pruning backups older than ${retention_days} day(s) in ${output_dir}\"\necho \"Backup complete: ${backup_file}\"\n\n";

    // Execute locally or remote? 
    // Two ways to provide parameters: 1. direct to method and 2. via options object
    // Standardized IScriptOutput for both remote and local execution, local must support multiple shells
    public IScriptOutput Execute()
    {
        throw new NotImplementedException();
    }
}