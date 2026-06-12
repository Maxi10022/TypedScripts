# Argument Definition

::: error
Complete this documentation page with codegen samples for completeness! Especially the Samples section!
:::

TypedScripts supports positional arguments and named arguments, it uses positional args by default.
When writing a script which should be called via TypedScripts you must define args as shown below.

::: warning
Arguments must be parsed by the script itself, TypedScripts only passes the arguments as defined by the script. 
:::

## Syntax

Defined arguments are the backbone of the generated strongly-typed C# script interface.
Make sure to follow the defined argument-definition syntax correctly.

`# @param {paramName}:{paramType} {required|optional} {default=<value>?} {argName=<name>?}`

### Syntax breakdown

* `paramName`: Parameter name used in the C# method.
* `paramType`: One of the supported C# types, [see here](/supported-argument-types).
* `required|optional` ***(Optional)***: Mark argument as `required` for mandatory args; omit or use `optional` for non-mandatory arguments.
* `default=<value>?` ***(Optional)***: Set a default value for this argument. Quote the value if it contains whitespaces.
* `argName=<name>?` ***(Optional)***: Named argument used when calling the script, otherwise defaults to positional args.

### Syntax definition requirements
* An argument definition must always live in its own line.
* Inlining multiple argument definitions is not supported, neither is inlining with code.

## Argument behaviour 

By default arguments will be passed as positional args in order of their detection, this means from top to bottom.
As already mentioned arguments must be parsed by the script itself.
This section breaks down how both argument-types are used when calling the script, which is important information when you are writing a script. 

### Positional arguments

TypedScripts uses positional arguments by default, which makes it easy to parse them. 
Their order is determined based upon when it was read by the parser, which reads from *top to bottom*. 
Each positional argument is passed to the script in the order it was defined: the first argument in your file becomes the first CLI argument, the second becomes the second, etc.

When an optional argument is empty or set to null, its value is set to an emtpy string `""` when executing the script.
This prevents breaking index based argument parsing.

### Named arguments

Named arguments let you define parameters invoked with a key-value syntax.
All named arguments come *after* all positional arguments in the CLI.
If a named argument is not provided, it is omitted entirely from the script execution

## Samples

### Positional Arguments

**Example 1: Required arguments**

```bash
#!/bin/bash
# @param name:string required
# @param age:int required

echo "Hello $1, you are $2 years old"
```

Called as: `script.sh "Alice" 30`

---

**Example 2: Mix of required and optional arguments**

```bash
#!/bin/bash
# @param inputFile:string required
# @param outputFile:string optional default="output.txt"
# @param verbose:bool optional default=false

echo "Processing $1"
if [ "$3" = "true" ]; then
  echo "Verbose mode enabled"
fi
echo "Output will be written to $2"
```

Called as (C# passes defaults to the script):
- `script.sh "input.txt" "output.txt" false` (C# passes default values)
- `script.sh "input.txt" "custom.txt" true` (C# passes provided values)

---

### Named Arguments

**Example 3: Named arguments only**

```bash
#!/bin/bash
# @param environment:string required argName=env
# @param region:string optional default="us-east-1" argName=region
# @param dryRun:bool optional default=false argName=dry-run

echo "Deploying to $1 region: $2"
if [ "$3" = "true" ]; then
  echo "DRY RUN - no changes will be made"
fi
```

Called as:
- `script.sh --env=production` (region and dryRun named args are omitted)
- `script.sh --env=staging --region=eu-west-1 --dry-run=true` (all named args provided)

---

**Example 4: Mixing positional and named arguments**

```bash
#!/bin/bash
# @param action:string required
# @param force:bool optional default=false argName=force
# @param retries:int optional default=3 argName=retries
# @param target:string required

echo "Action: $1, Target: $2"
echo "Force: --force=$3, Retries: --retries=$4"
```

Called as:
- `script.sh deploy api` (positional only, named args use defaults)
- `script.sh deploy api --force=true --retries=5` (mixed)

---

### Handling Empty/Null Values

- **Positional arguments with defaults:** Default values are passed as arguments from C# to the script.
- **Named arguments without a value:** Empty named arguments are omitted entirely from the script execution.
- **Positional arguments without defaults:** Unprovided optional positional arguments are passed as empty strings `""` to maintain index-based parsing.

```bash
#!/bin/bash
# @param message:string required
# @param prefix:string optional default="[LOG]" argName=prefix

echo "$1: $2"
```

Called as:
- `script.sh "Hello"` → receives `"Hello" "[LOG]"` (default passed)
- `script.sh "Hello" --prefix="INFO"` → receives `"Hello" "INFO"`
- `script.sh "Hello"` with no named prefix → default is still passed from C#

## Recommendations

It is generally recommended to define all arguments at the top of your script. 
This makes them easier to maintain in the long run and gives other developers a better overview of the scripts requirements.