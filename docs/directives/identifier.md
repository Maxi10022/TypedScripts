# Class name

`@identifier` sets the name of the generated C# class.

::: info
Samples use the `#` leader for brevity, see [supported leaders](/directives/#comment-leaders).
:::

```text
# @identifier <identifier>
```

| Part         | Required | Notes                                                                       |
|--------------|----------|-----------------------------------------------------------------------------|
| `identifier` | Yes      | Must be a valid C# identifier. Reserved keywords are escaped automatically.  |

```bash
# @identifier BackupDatabase   # -> public class BackupDatabase
```

## Fallback to the file name

When no `@identifier` is present, the class name is derived from the **file name**: it is split on any
run of non-alphanumeric characters and each segment is PascalCased (inner casing is preserved).

| File name             | Generated class            |
|-----------------------|----------------------------|
| `backup-database.sh`  | `BackupDatabase`           |
| `myWeb-deploy.sh`     | `MyWebDeploy`              |
| `1stPlace.sh`         | *(invalid — see below)*    |

The same fallback applies when an `@identifier` is present but **invalid**: the bad value is reported
as [`TSC002`](/reference/diagnostics) and TypedScripts falls back to the file-name identifier.

## Reserved keywords

A value that is a C# reserved keyword is escaped automatically with a leading `@`:

```bash
# @identifier class   # -> public class @class
```

## When the name can't be resolved

The file-name fallback can itself be invalid — for example `1stPlace.sh` becomes `1stPlace`, which is an invalid C# identifier. 
Then the script is **skipped** and reported as [`TSC006`](/reference/diagnostics).

## Multiple directives

If a script contains more than one `@identifier`, the **last** one wins.

## Diagnostics

| ID                                 | Severity | Raised when                                                                |
|------------------------------------|----------|----------------------------------------------------------------------------|
| [`TSC002`](/reference/diagnostics) | Warning  | The `@identifier` value is not a valid C# identifier.                       |
| [`TSC006`](/reference/diagnostics) | Warning  | No valid identifier could be resolved (directive and file name both invalid). |
