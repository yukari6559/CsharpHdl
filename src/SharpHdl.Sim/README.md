# SharpHdl.Sim

Cycle-style simulation for [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) modules (`Run` / `Set` / `Get` / `Settle` / `Advance`).

**Current scope (S2):** Comb + Seq, sync memories (1R1W / 2R1W / byte-strobe), one level of module hierarchy, `LoadMem` / `PeekMem` / `PokeMem`, `AdvanceWhile`. Assert with ordinary xUnit.

## Install

```xml
<PackageReference Include="SharpHdl.Core" Version="0.4.0" />
<PackageReference Include="SharpHdl.Sim" Version="0.4.0" />
```

Optional with emit:

```xml
<PackageReference Include="SharpHdl.Emit" Version="0.4.0" />
```

## Docs

- Repository: https://github.com/yukari6559/CsharpHdl
- Consumer guide: https://github.com/yukari6559/CsharpHdl/blob/main/docs/consumers.md

## License

MIT
