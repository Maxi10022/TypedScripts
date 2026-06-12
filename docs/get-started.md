# Get Started With TypedScripts

TypedScripts makes shell scripts strongly typed objects with a clean execution interface.
It works best when used in its idiomatic approach. When functionality needs to be expanded, pull requests are always welcome and quick to be addressed.

**Install the package**

```bash
nuget install TypedScripts
```

**Make your script parseable**

TypedScripts works by honoring a few basic convetions.

1. Set script compile-property to `Additional Text` 
2. Define parameters via comments, one line one parameter
3. Parse arguments yourself

Here is a simple example:

```bash
#!/bin/bash
# @param date:string required
# @param port:int default=8080
# @param verbose:bool

date_value="$1"
port_value="${2}"
verbose="${3:-false}"

echo "Running on date: $date_value"
echo "Port: $port_value"
echo "Verbose mode: $verbose"
```

> Notice the `required` and `default` parameter flags? Those are handled via C#.

**Execute your script**