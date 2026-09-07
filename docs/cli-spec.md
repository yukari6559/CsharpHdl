# CLI 仕様 v0.1

`SharpHdl.Cli` が提供するコマンドの契約（実装は人間）。

## 位置づけ（重要）

CLI は **補助**。examples のデモ・Quick Start・CI 向けの薄い出口。

本線は消費者が Core / Emit を参照し、自分の `Module` を C# で書いて Emitter を呼ぶこと（[consumers.md](consumers.md)）。  
名前だけの `emit` では、C# コンパイルの恩恵はほぼ SharpHdl 側のビルド時に使い切り済み。

将来、DLL＋型名で消費者アセンブリを読み込む形に広げてもよい（その場合も、コンパイルは消費者の `dotnet build` が本丸）。

## コマンド

### emit

```bash
dotnet run --project src/SharpHdl.Cli -- emit <input> -o <output.v> [options]
```

| 引数 | 意味 |
|------|------|
| `input` | 既知 example のキー（実装で決める。例: `alu` / `simple-ram`） |
| `-o` | 出力 Verilog パス |
| `--top Name` | トップモジュール名（任意） |
| `--sv` | （将来）SystemVerilog |

**完了条件（補助）**: 終了コード 0 でファイルが書かれる。Verilator パースは任意〜回帰。

### list（任意）

利用可能な例モジュールを列挙。

### version

ツールバージョン表示。

## 入力の形態（v0.1）

**推奨**: `examples/` 相当の Module をリポジトリ内に置き、CLI は既知一覧から選んでインスタンス化・emit。

後回し:

- スクリプト的な `.cs` のその場コンパイル  
- 任意 DLL＋型名（本線が通ってから検討）

## エラー

| 状況 | 終了コード |
|------|------------|
| 成功 | 0 |
| 引数不正 | 2 |
| emit 失敗（幅不一致等） | 1 |
