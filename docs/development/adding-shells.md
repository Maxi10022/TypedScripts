# Adding new shells

There are two parts to adding a new shell.

1. An implementation of `IShellSession` which takes care of **stdin** **stdout** and **stderr** streams.
2. An implementation of `IShellExecutor` which takes care of actually running the shell script, returns the custom `IShellSession` implementation.

## Example based on CMD for Windows

As the title already says, here is a commented example for a Windows CMD executor.

