# examples/counter

Phase 2 の同期カウンタ例。`Seq` で 8 ビットカウンタを記述する。

- `step` を `8'd1` に固定すれば毎クロック +1
- リセット時は `count <= 0`

検証は `SharpHdl.Tests` の `TestCounterModuleEmitter_EmitsSeq` を参照。

```bash
dotnet test --filter TestCounterModuleEmitter_EmitsSeq
```
