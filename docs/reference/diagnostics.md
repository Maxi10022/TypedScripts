# Diagnostics

Every problem TypedScripts reports is a **warning** — generation never fails the build. Depending on
where the problem is, TypedScripts either drops the offending piece of configuration (a bad `@param`)
or skips the whole script (no usable identifier or interpreter).

## Script diagnostics (`TSC`)

| ID       | Title                                     | Severity | Raised when                                                                                      |
|----------|-------------------------------------------|----------|--------------------------------------------------------------------------------------------------|
| `TSC001` | Failed to read script content             | Warning  | The file was detected but its text couldn't be read.                                             |
| `TSC002` | Invalid script identifier                 | Warning  | An [`@identifier`](/directives/identifier) value is not a valid C# identifier. Falls back to the file name. |
| `TSC003` | Invalid script syntax                     | Warning  | The generated C# failed to parse (internal safety net).                                          |
| `TSC004` | No interpreter available                  | Warning  | No [interpreter](/directives/interpreter) resolved from directive, shebang or extension. Script skipped. |
| `TSC005` | Interpreter not supported                 | Warning  | A directive or shebang names an unsupported interpreter.                                          |
| `TSC006` | Unknown problem during script validation  | Warning  | An unexpected error while building the script — e.g. the file-name fallback identifier is also invalid. Script skipped. |

## Argument diagnostics (`TSARG`)

| ID         | Title                                       | Severity | Raised when                                                          |
|------------|---------------------------------------------|----------|---------------------------------------------------------------------|
| `TSARG001` | Unsupported argument type                   | Warning  | A [`@param`](/directives/param) uses a type that isn't [supported](/reference/supported-types). |
| `TSARG002` | Invalid default value                       | Warning  | A `default=` value can't be parsed as the declared type.            |
| `TSARG003` | Invalid parameter identifier                | Warning  | A `@param` name isn't a valid C# identifier.                        |
| `TSARG004` | Unsupported argument default                | Warning  | A default was supplied for a type that accepts none. *(Not reachable with the current type set.)* |
| `TSARG005` | Unknown problem during argument validation  | Warning  | An unexpected error while validating a parameter.                   |

::: info
A directive that is *structurally* malformed (for example a missing or misspelled head) doesn't
produce a diagnostic at all — the line simply [doesn't match](/directives/#parse-rules) and is skipped.
:::
