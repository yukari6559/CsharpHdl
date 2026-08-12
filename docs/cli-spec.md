# CLI 仕様 v0.1

`SharpHdl.Cli` が提供するコマンドの契約（実装は人間）。

## コマンド

### emit

```bash
dotnet run --project src/SharpHdl.Cli -- emit <input> -o <output.v> [options]
```

| 引数 | 意味 |
|------|------|
| `input` | 記述の入口（例: アセンブリ名、または examples パス）。実装で決める |
| `-o` | 出力 Verilog パス |
| `--top Name` | トップモジュール名 |
| `--sv` | （将来）SystemVerilog |

**完了条件**: 終了コード 0 でファイルが書かれ、Verilator がパースできる。

### list（任意）

利用可能な例モジュールを列挙。

### version

ツールバージョン表示。

## 入力の形態（実装選択）

次のいずれか（または両方）:

1. **ライブラリ内の Module 型を指定**してインスタンス化・emit  
2. **スクリプト的な C# ファイル**を読み込み（高度・後回し）  

v0.1 推奨: 例は `examples/` に Module クラスとして置き、CLI は既知一覧から選ぶ。

## エラー

| 状況 | 終了コード |
|------|------------|
| 成功 | 0 |
| 引数不正 | 2 |
| emit 失敗（幅不一致等） | 1 |
