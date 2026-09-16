# SharpHdl.Emit

Verilog emitter for modules built with [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core).

## Install

```xml
<PackageReference Include="SharpHdl.Core" Version="0.3.1" />
<PackageReference Include="SharpHdl.Emit" Version="0.3.1" />
```

## Usage

Call `VerilogEmitter.Emitter(module, moduleName)` (or the project’s emit entry points) after describing a `Module`, then write the returned string to a `.v` file.

## Docs

- Repository: https://github.com/yukari6559/CsharpHdl
- Consumer guide: https://github.com/yukari6559/CsharpHdl/blob/main/docs/consumers.md

## License

MIT
