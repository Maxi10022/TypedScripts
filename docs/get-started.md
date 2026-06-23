# Get Started With TypedScripts

TypedScripts makes shell scripts strongly-typed objects with a clean execution interface.
It works best when used in its idiomatic approach. When functionality needs to be expanded, pull
requests are always welcome and quick to be addressed.

**Install the package**

```bash
nuget install TypedScripts
```

**Make your script discoverable**

TypedScripts works by honoring a few basic conventions:

1. Set the script's **Build Action** to **C# analyzer additional file** (`AdditionalFiles`).
2. Give it a **`.sh`** extension.
3. Define parameters via [directives](/directives/) — one per line.
4. Parse the arguments inside the script yourself.

```xml
<ItemGroup>
  <AdditionalFiles Include="Scripts/example.sh" />
</ItemGroup>
```

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

> Notice the `required` and `default` flags? Those are handled in C#, not by the script.

**What you get**

Each detected script is turned into a C# class named after the file (or its
[`@identifier`](/directives/identifier)). Today the generated class embeds the script body; the typed
constructor and execution API are in active development.

## Next steps

* [Parameters](/directives/param) — declare strongly-typed inputs with `@param`.
* [Class name](/directives/identifier) — control the generated class name.
* [Interpreters](/directives/interpreter) — pick the shell via directive, shebang or extension.
