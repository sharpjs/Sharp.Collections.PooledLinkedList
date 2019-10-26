# Sharp.Collections.PooledLinkedList

A .NET singly-linked list data structure implemented using a node pool for
improved cache performance relative to a traditional linked list.

## Status

Work in progress.

## Building

Requirements:
- [.NET Framework 4.8 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework)
- [.NET Core 3.0 SDK](https://dotnet.microsoft.com/download/dotnet-core)
- Maintain 100% test coverage
- Maintain 100% documentation coverage

Build with Visual Studio 2019, or from the command line:

```powershell
# The default: build and run tests
Make.ps1 [-Test] [-Configuration <String>]

# Just build; don't run tests
Make.ps1 -Build [-Configuration <String>]

# Build and run tests w/coverage
Make.ps1 -Coverage [-Configuration <String>]
```

## References

* [.NET Standard FAQ](https://github.com/dotnet/standard/blob/master/docs/faq.md)
* [.NET Standard Versions](https://github.com/dotnet/standard/blob/master/docs/versions.md)
* [Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)
