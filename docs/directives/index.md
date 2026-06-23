# Directives

TypedScripts configures each generated class from **directives** — single-line comments inside your
script. A directive starts with a comment leader (such as `#`), followed by an `@`-prefixed keyword
and its value:

```bash
# @identifier BackupDatabase
# @interpreter bash
# @param database:string required
```

The samples throughout these docs use the `#` leader for brevity, but TypedScripts recognizes the
comment styles of many languages, so the same directives work regardless of the script's syntax.

| Directive      | Purpose                                      | Page                              |
|----------------|----------------------------------------------|-----------------------------------|
| `@param`       | Define a strongly-typed script parameter     | [Parameters](/directives/param)   |
| `@identifier`  | Set the generated C# class name              | [Class name](/directives/identifier) |
| `@interpreter` | Select the interpreter the script runs with  | [Interpreters](/directives/interpreter) |

## How a script is picked up

Before any directive is read, the file has to be visible to the generator:

* Its **Build Action** must be **C# analyzer additional file** (`AdditionalFiles` in the `.csproj`).
* Its path must end in **`.sh`**.

```xml
<ItemGroup>
  <AdditionalFiles Include="Scripts/backup-database.sh" />
</ItemGroup>
```

Files that don't meet both conditions are ignored silently — no class and no diagnostic. (The
`.bash` extension maps to Bash during [interpreter inference](/directives/interpreter#file-extension),
but only `.sh` files are currently detected.)

## Comment leaders

A directive is recognized after any of the following leaders. 
Leading indentation is allowed, and the space between the leader and the `@directive` is optional.

| Leader | Typical languages           |
|--------|-----------------------------|
| `#`    | Shell, Python, PowerShell   |
| `//`   | JavaScript, TypeScript      |
| `--`   | SQL, Lua                    |
| `;`    | Lisp, INI                   |
| `%`    | MATLAB, LaTeX               |
| `::`   | Batch                       |
| `REM`  | Batch                       |

::: info
The leader and the directive keyword are matched **case-insensitively**, so `REM`, `rem`, `# @PARAM`
and `# @Param` are all valid.
:::

All of these are equivalent:

```text
# @interpreter bash
    // @interpreter bash
-- @interpreter bash
REM @interpreter BASH
```

## Parse rules

* **One directive per line.** A directive must occupy its own line. You cannot combine two directives,
  or mix a directive with code, on the same line.
* **The line must match the directive's shape.** A directive is recognized only when the line matches
  its expected shape. For `@param` that means a well-formed `paramName:paramType` head — its optional
  parts may appear in any order and unrecognized tokens are ignored. A line that doesn't match at all
  is **silently skipped**, not reported.
* **Recognized but invalid values are diagnosed.** When a directive matches but its value is invalid
  (an unsupported type, an unparseable default, a non-identifier name), a
  [diagnostic](/reference/diagnostics) is emitted and that piece of configuration is dropped.

The distinction matters: a typo in the *structure* of a directive makes it disappear without a
warning, while a bad *value* in an otherwise well-formed directive is surfaced as a diagnostic.

## What gets generated

Each detected script becomes one C# class in a file named `<Identifier>.g.cs`. Today the generated
class embeds the script body as a constant:

```csharp
// BackupDatabase.g.cs (generated)
public class BackupDatabase
{
    private const string ScriptContent = "#!/bin/bash\n# @param database:string required\n...";
}
```

Parameters declared with `@param` are parsed and validated at this stage, but are not yet surfaced on
the class — turning them into constructor parameters and execution methods is in active development.
