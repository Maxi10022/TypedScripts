# Interpreters

An interpreter tells TypedScripts which shell a script runs with. It can come from a directive, a
shebang, or the file extension. Currently **Bash** is the only supported interpreter.

::: info
Samples use the `#` leader for brevity — see [supported leaders](/directives/#comment-leaders).
:::

## `@interpreter`

```text
# @interpreter <interpreter>
```

The value is matched **case-insensitively** against the supported interpreters. 
An unsupported value is reported as [`TSC005`](/reference/diagnostics) and ignored.

```bash
# @interpreter bash
# @interpreter BASH   # same thing
```

## Shebang

A standard shebang on the **first line** is also honoured:

```bash
#!/bin/bash
#!/usr/bin/env bash
```

* The interpreter is taken from the final path segment (`/bin/bash` → `bash`).
* The `env` form resolves to its following argument (`/usr/bin/env bash` → `bash`).
* A space after the marker (`#! /bin/bash`) and trailing flags (`#!/bin/bash -e`) are tolerated.
* The shebang is **only** read on the first line; a `#!` anywhere else is treated as an ordinary comment.

An unsupported shebang interpreter is reported as [`TSC005`](/reference/diagnostics).

## File extension

If neither a directive nor a shebang names an interpreter, the file extension is used:

| Extension | Interpreter |
|-----------|-------------|
| `.sh`     | Bash        |
| `.bash`   | Bash        |

::: info
Only `.sh` files are [detected](/directives/#how-a-script-is-picked-up) today, so the `.bash` mapping
is reached only if detection is later widened.
:::

## Resolution

The interpreter is **not** a single winner-takes-all choice. Sources combine like this:

1. Every `@interpreter` directive **and** a first-line shebang each contribute an interpreter, in the
   order they appear in the file.
2. The file extension is used **only as a fallback**, when neither a directive nor a shebang produced
   any interpreter.

If none of these yield a supported interpreter, the script is skipped and reported as
[`TSC004`](/reference/diagnostics).

## Diagnostics

| ID                                 | Severity | Raised when                                                            |
|------------------------------------|----------|------------------------------------------------------------------------|
| [`TSC004`](/reference/diagnostics) | Warning  | No interpreter could be resolved from directive, shebang or extension. |
| [`TSC005`](/reference/diagnostics) | Warning  | A directive or shebang names an unsupported interpreter.               |
