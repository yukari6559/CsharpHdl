# SharpHdl.Sim

Cycle-style simulation for [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) modules (`Run` / `Set` / `Get` / `Settle` / `Advance`).

**Current scope (S2):** Comb + Seq, sync memories (1R1W / 2R1W / byte-strobe), one level of module hierarchy, `LoadMem` / `PeekMem` / `PokeMem`, `AdvanceWhile`, local `Wire` / `Reg` (observable with `Get`; `Set` from the testbench throws). Assert with ordinary xUnit.

## Install

```xml
<PackageReference Include="SharpHdl.Core" Version="0.11.0" />
<PackageReference Include="SharpHdl.Sim" Version="0.11.0" />
```

Optional with emit:

```xml
<PackageReference Include="SharpHdl.Emit" Version="0.11.0" />
```

## Docs

- Repository: https://gitlab.com/sharphdl/SharpHDL
- Documentation and API reference: https://sharphdl.gitlab.io/SharpHDL
- Consumer guide: https://gitlab.com/sharphdl/SharpHDL/-/blob/main/docs/consumers.md

## License

MIT
