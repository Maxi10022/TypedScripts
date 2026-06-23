# Parameters

The `@param` directive declares a single script parameter. Each parameter becomes a strongly-typed
member of the generated class and is validated at build time.

::: info
Samples use the `#` leader for brevity — see [supported leaders](/directives/#comment-leaders).
:::

```text
# @param <paramName>:<paramType> [required|optional] [default=<value>] [argName=<name>]
```

| Part                    | Required | Notes                                                                                              |
|-------------------------|----------|----------------------------------------------------------------------------------------------------|
| `paramName`             | Yes      | The C# parameter name. Must be a valid C# identifier; reserved keywords are escaped automatically (`class` → `@class`). |
| `paramType`             | Yes      | One of the [supported argument types](/reference/supported-types). Matched case-insensitively.     |
| `required` / `optional` | No       | `required` produces a non-nullable parameter; omitting it (or writing `optional`) produces a nullable one. |
| `default=<value>`       | No       | Sets a default value. Quote it if it contains whitespace.                                          |
| `argName=<name>`        | No       | Turns the parameter into a [named argument](#named-vs-positional-arguments). Hyphens are allowed (`argName=dry-run`). |

```bash
# @param database:string required
# @param outputDir:string optional default="/var/backups"
# @param port:int optional default=5432
# @param compress:bool optional default=true
# @param dryRun:bool optional default=false argName=dry-run
```

::: tip Order doesn't matter
The optional parts — `required`/`optional`, `default=` and `argName=` — may appear in any order. The
form above is just the conventional one. Tokens that aren't recognized are ignored rather than
breaking the directive. See [parse rules](/directives/#parse-rules).
:::

## required vs optional

`required` and `optional` control **nullability only**:

* `required` → non-nullable (`string`)
* `optional` or omitted → nullable (`string?`)

They are independent of `default=`. A `required` parameter can still carry a default value — the type
stays non-nullable and the default is attached:

```bash
# @param database:string required default=postgres   # -> string database = "postgres"
```

## Default values

`default=` accepts a bare token or a `"quoted value"` (use quotes when the value contains spaces).
A few behaviours are worth knowing:

* **`default=null` and `default=""` both yield `null`.** An empty or whitespace-only value is treated
  as the null literal, regardless of the parameter type — so `default=""` does *not* produce an empty
  string.
* **Numeric and `char` defaults are validated.** The value is parsed as the declared type; a value
  that doesn't parse (`default=abc` on an `int`, or a multi-character `char`) is reported as
  [`TSARG002`](/reference/diagnostics).
* **`bool` defaults are lenient.** `true` (any casing) becomes `true`; **anything else becomes
  `false`** without a diagnostic — so `default=yes` silently means `false`.

## Named vs positional arguments

`@param` decides how an argument will be passed to the script:

* **Positional** (the default) — passed in declaration order.
* **Named** — produced by adding `argName=<name>`.

::: info Execution is in progress
Directives, including `argName`, are fully parsed and validated today. The runtime that actually
launches the script and forwards these arguments is still being built; the rules below describe the
intended model.
:::

The intended rules:

* Positional arguments keep their **declaration order**.
* Positional arguments always come **before** named arguments.
* An empty **positional** argument is still passed, as an empty string.
* An empty **named** argument is omitted entirely.

Your script is always responsible for reading and parsing the arguments it receives.

Given:

```bash
# @param inputFile:string required
# @param verbose:bool default=false
# @param outputFile:string argName=out
```

the script would be invoked roughly as:

```bash
bash ./script.sh <inputFile> <verbose> --out=<outputFile>
```

## Supported types

`string`, `char`, `bool`, the signed integers (`sbyte`, `short`, `int`, `long`), the unsigned integers
(`ushort`, `uint`, `ulong`) and the floating-point/decimal types (`float`, `double`, `decimal`). An
unsupported type is reported as [`TSARG001`](/reference/diagnostics). See the full
[supported types reference](/reference/supported-types) for grouping and notes — for example, `byte`
is **not** supported, only `sbyte`.

## Diagnostics

| ID                                   | Severity | Raised when                                              |
|--------------------------------------|----------|----------------------------------------------------------|
| [`TSARG001`](/reference/diagnostics) | Warning  | The `paramType` is not a supported type.                 |
| [`TSARG002`](/reference/diagnostics) | Warning  | A `default=` value cannot be parsed as the declared type.|
| [`TSARG003`](/reference/diagnostics) | Warning  | The `paramName` is not a valid C# identifier.            |

A `@param` line is recognized as long as its `paramName:paramType` head is well-formed; the optional
parts may appear in any order and unrecognized tokens are ignored without a diagnostic. A line whose
head doesn't match at all (missing name, colon or type) isn't treated as a `@param` directive and is
simply skipped.
