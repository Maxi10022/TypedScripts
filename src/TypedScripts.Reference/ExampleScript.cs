using System.Threading.Tasks;
using Renci.SshNet;
using TypedScripts.Exceptions;
using TypedScripts.Executors;
using TypedScripts.Output;

namespace TypedScripts.Reference;

// Note for code-gen, always write out namespace before type references! 
// Allow users to instantiate by args object directly - skip fluent builder if preferred
public readonly struct ExampleScript(ExampleScriptArgs args)
{
    // The actual script as a string
    private const string ScriptContent = "#!/bin/bash\n\n# @identifier ExampleScript\n# @param database:string required\n# @param outputDir:string optional default=\"/var/backups\"\n# @param port:int optional default=5432\n# @param compress:bool optional default=true\n# @param retentionDays:int optional default=7\n\nset -euo pipefail\n\n# Positional arguments, read in declaration order. A value is always present at\n# each index because C# passes the @param default (or \"\") for optional args.\ndatabase=\"$1\"\noutput_dir=\"$2\"\nport=\"$3\"\ncompress=\"$4\"\nretention_days=\"$5\"\n\ntimestamp=\"$(date +%Y%m%d_%H%M%S)\"\nbackup_file=\"${output_dir}/${database}_${timestamp}.sql\"\n\necho \"Backing up database '${database}' (port ${port})\"\necho \"Target: ${backup_file}\"\n\nif [ \"$compress\" = \"true\" ]; then\n  backup_file=\"${backup_file}.gz\"\n  echo \"Compression enabled -> ${backup_file}\"\nfi\n\necho \"Pruning backups older than ${retention_days} day(s) in ${output_dir}\"\necho \"Backup complete: ${backup_file}\"\n\n";
    
    // Array of supported interpreters which can be used to execute this script.
    public static readonly Interpreter[] Interpreters = [ Interpreter.Bash ];

    // Fluent builder factory method taking in required parameters directly
    // Required parameters with default values are ordered last with default value set inline.
    public static ExampleScript Call(string database) => 
        new(new ExampleScriptArgs
        {
            Database = database
        });
    
    // START: Fluent build configuration methods
    public ExampleScript Database(string value)
    {
        args.Database = value;
        return this;
    }

    public ExampleScript OutputDir(string? value)
    {
        args.OutputDir = value;
        return this;
    }

    public ExampleScript Port(int value)
    {
        args.Port = value;
        return this;
    }
    // END: Fluent builder configuration methods
    
    // Execute script locally using the provided interpreter.
    public Task<IExecutionOutput> ExecuteLocalAsync(Interpreter interpreter = Interpreter.Bash)
    {
        ValidateArgs();
        var executor = ResolveLocalExecutor(interpreter);
        var options = new ExecutionOptions(ScriptContent, interpreter, args);
        return executor.ExecuteAsync(options);
    }
    
    // Execute the script on a remote machine via SSH using the provided interpreter.
    // `InterpreterNotSupportedException` and `RequiredParameterUnsetException` are expected exceptions
    public Task<IExecutionOutput> ExecuteRemoteAsync(SshClient client, Interpreter interpreter = Interpreter.Bash)
    {
        ValidateArgs();
        var executor = ResolveRemoteExecutor(interpreter, client);
        var options = new ExecutionOptions(ScriptContent, interpreter, args);
        return executor.ExecuteAsync(options);
    }

    // Resolve REMOTE executor instance or throw if not supported
    // `InterpreterNotSupportedException` and `RequiredParameterUnsetException` are expected exceptions
    private IExecutor ResolveRemoteExecutor(Interpreter interpreter, SshClient client) => 
        interpreter switch
        {
            Interpreter.Bash => new BashSshExecutor(client),
            _ => throw new InterpreterNotSupportedException(interpreter, "ExampleScript")
        };

    
    // Resolve LOCAL executor instance or throw if not supported
    private IExecutor ResolveLocalExecutor(Interpreter interpreter) => 
        interpreter switch
        {
            Interpreter.Bash => new BashLocalExecutor(),
            _ => throw new InterpreterNotSupportedException(interpreter, "ExampleScript")
        };

    // Validates all required arguments are set. 
    private void ValidateArgs()
    {
        if (args.Database is null)
            throw new RequiredParameterUnsetException(argName: "Database", argType: "ExampleScriptArgs");
    }
}