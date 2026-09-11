# examples/alu-top

Phase 3 の階層モジュール例。`AluTop` が子 `Alu` を1つインスタンス化します。

- `AluModule.cs` — 子モジュール（`examples/alu` と同等）
- `AluTopModule.cs` — 親モジュール（ポートを子に接続）

```bash
dotnet test src/SharpHdl.Tests --filter AluTop
```

生成イメージ: 1ファイルに `module Alu` → `module AluTop`（インスタンス行付き）が連結されます。
