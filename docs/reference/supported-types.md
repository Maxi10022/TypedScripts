# Supported argument types

These are the types accepted by [`@param`](/directives/param). Type names are matched
case-insensitively and normalized to lowercase, so `String`, `int` and `INT` are all fine.

**Text**
* `string` — text
* `char` — a single character

**Boolean**
* `bool`

**Signed integers**
* `sbyte`, `short`, `int`, `long`

**Unsigned integers**
* `ushort`, `uint`, `ulong`

**Floating-point & decimal**
* `float`, `double`, `decimal`

::: info Not supported
Native-sized integers (`nint`, `nuint`),
`object`, enums and custom types are unsupported as well. An unrecognized type is reported as
[`TSARG001`](/reference/diagnostics).
:::