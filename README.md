# Welcome ty TypedScripts!

TypedScripts brings strongly-typed bash scripts to your C# project.

## Quick Start

1. Install the nuget package

```bash
nuget install TypedScripts
```

Or use your IDEs built in nuget manager.

2. Make your bash scripts parseable

At the top of your script define arguments like so:

```bash
#!/bin/bash
# @param date:string --date required
# @param port:int --port default=8080
# @param verbose:bool --verbose
```

> You must still parse arguments yourself!

3. Run the script from C#

