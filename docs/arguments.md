# Argument Reference

In TypedScripts arguments are parsed from comments located in the script, added by the developer who writes that script. 
The detected arguments are then used to generate a strongly-typed interface for script execution from C#.  

## Defining arguments 

You can define script arguments for the generated C# interface using the following comment structure:

`# @param {paramName}:{paramType} {required|optional} {default=<value>?} {argName=<name>?}`

* `paramName`: Parameter name used in the C# method.
* `paramType`: One of the supported C# types, [see here](/supported-argument-types).
* `required|optional` ***(Optional)***: Mark argument as `required` for mandatory args; omit or use `optional` for non-mandatory arguments.
* `default=<value>?` ***(Optional)***: Set a default value for this argument. Quote the value if it contains whitespaces.
* `argName=<name>?` ***(Optional)***: Named argument used when calling the script, otherwise defaults to positional args.

::: warning
Arguments must be parsed by the script itself, TypedScripts only passes the arguments as defined during execution. 
:::

## Further rules 

In order for TypedScripts to correctly pick up your arguments make sure you follow these two simple rules:

* An argument definition must always live in its own line.
* Inlining multiple argument definitions is not supported, neither is inlining with code.
 
## Arguments in execution

TypedScripts supports two types of arguments:

* **Positional arguments** (the default)
* **Named arguments**

Those two argument-types also have some rules applied to them:

* *Positional* arguments are set in the same order as they were discovered. 
* *Positional* arguments always come **before** named *arguments*.
* Empty *positional* arguments are still passed as empty strings
* Empty *named* arguments are omitted

To understand those rules better, lets walk through a simple sample.

```bash
# @param inputFile:string required
# @param outputFile:string argName=out
# @param verbose:bool default=false
```

// TODO update the sample here once the actual implementation stands!
The generated C# class would look like this:

```csharp
public class ExampleScript(string inputFile, string? outputFile, bool? verbose = false) 
{ 
    ... 
}
```

Your script will then get executed like this:

```bash
bash ./script.sh input.txt false --out=backup.sql 
```