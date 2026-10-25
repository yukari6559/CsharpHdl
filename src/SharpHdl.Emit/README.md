# SharpHdl.Emit

Verilog emitter for modules built with [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core).

## Install

```xml
<PackageReference Include="SharpHdl.Core" Version="0.8.0" />
<PackageReference Include="SharpHdl.Emit" Version="0.8.0" />
```

## Usage

Call `VerilogEmitter.Emitter(module, moduleName)` (or the project’s emit entry points) after describing a `Module`, then write the returned string to a `.v` file.

Comb emit shape (current): `Switch` → continuous `assign` ternary (`output wire`); `If` → `always @(*)` procedural assigns (`output reg` on driven outs).

Mem array names: optional `name` on `Mem`; default `mem_{rdata}` (2R1W: `mem_{rdata0}`).

## Docs

- Repository: https://gitlab.com/sharphdl/SharpHDL
- Documentation and API reference: https://sharphdl.gitlab.io/SharpHDL
- Consumer guide: https://gitlab.com/sharphdl/SharpHDL/-/blob/main/docs/consumers.md

## License

MIT
