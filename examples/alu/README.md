# examples/alu

Phase 1 のテキストビルダー試作。

```bash
dotnet run --project src/SharpHdl.Cli -- emit alu
# → out/generated/alu.v
```

`AluModule.cs` は暫定的に `SharpHdl.Cli` からコンパイルされています（csproj の `Compile Include`）。
