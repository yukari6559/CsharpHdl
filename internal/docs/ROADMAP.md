# CsharpHdl ロードマップ

## ゴール

1. C# DSL で同期回路を記述できる  
2. 人が読める Verilog を生成できる  
3. **nuget.org**（または開発時のプロジェクト参照）で **他リポジトリから使える**  
4. 例: ALU、点滅、（消費者側で）CPU コア / MIDI シンセ  

## フェーズ

| Phase | 内容 | ガイド | 目安 |
|-------|------|--------|------|
| 0 | 方針・用語・DSL 範囲の確定 | [phase-0](guides/phase-0-foundation.md) | 数日〜1 週 |
| 1 | ALU を生成（StringBuilder でも可） | [phase-1](guides/phase-1-alu.md) | 1〜2 週 |
| 2 | AST + DSL + Emitter | [phase-2](guides/phase-2-dsl.md) | 2〜4 週 |
| 3 | Mem・階層・CPU 向け部品 | [phase-3](guides/phase-3-cpu-ready.md) | 3〜6 週 |
| 4 | nuget.org 配布（T1）・消費者 emit・CLI 補助 | [phase-4](guides/phase-4-tooling.md) | 1〜2 週 |
| **Sim** | C# シミュ（S1→S2） | [sim-spec.md](sim-spec.md) / [sim-impl.md](sim-impl.md) | **GS2 完了**（`0.3.5`） |
| **L1（MidiSynth）** | S2 上の原語穴埋め | [tickets/midisynth-tang-primer25k-requests.md](tickets/midisynth-tang-primer25k-requests.md) | M1–M5（**いまの本線**） |

## マイルストーン

| ID | 到達点 |
|----|--------|
| G0 | 手で書いた Verilog と同等の ALU を C# から出力 |
| G1 | `Module` DSL で ALU / カウンタ |
| G2 | 1R1W メモリ + 階層モジュール |
| G3 | nuget.org のパッケージで消費者から参照・ビルド（T1 完了・`0.1.0`） |
| G4 | examples が Verilator で通る |
| **GS2** | Sim **S2 完了**（Mem3種 + Instance + Load/Peek/Poke + AdvanceWhile）— **完了**（`0.3.5`） |
| **GM** | MidiSynth 宿題 **M1–M5** 完了 → 単音シンセがフォークなしで書ける |
| **GS1** | Stage 1 完了: 単音シンセ + C# の回路を VS Code から 1 操作で Tang Primer 25K に書き込める（HDL 到達度 約 32%） |
| **GS4** | Stage 4 完了: Verilog / SystemVerilog の機能マトリクス全行完了（10 割）。[hdl-coverage-plan.md](hdl-coverage-plan.md) |
| **GS6** | Stage 6: 半導体（ASIC）設計・検証へ発展。C# の回路がオープンな PDK で GDS まで通り、RTL・ゲートレベル・C# Sim が一致。既存 RTL を C# で検証できる（SX12、[hdl-coverage-plan.md](hdl-coverage-plan.md) §4.3） |

## いまの本線（MidiSynth L1）

```
S1（Comb+Seq）           … 完了
  → S2（Mem / 階層 / LoadMem / AdvanceWhile）  … 完了（0.3.5）
    → M1a 比較（Eq/Neq + 関係演算）           … 完了（0.3.6）
    → M1b If / Else（名前付き then / @else）  … 完了（0.3.7）
    → M1c Switch との役割分担を dsl-spec に明記 … 完了
    → M3 定数 … 完了（0.3.8）
    → refactor-visitor（Stmt／Expr Visitor、R-V1〜V4）… 完了（0.4.0）
    → M4a ローカル Wire / Reg（Core）… 完了（PR #85）
    → M4b = HC0 ローカル線の Emit / Sim… 完了（PR #100、#8。`0.5.0`）
    → M2 = HC1 演算子の穴埋め（乗算・シフト・^~ に加え、リダクション・論理・Mux・繰り返し連結）（**進行中**）
        XOR / XNOR … 完了（PR #103。`0.5.0`）
        論理シフト `<<` / `>>` … 完了（M2b）
        → 先に正しさの修正: #102 括弧（済み）→ #105 左右の幅（済み）→ #104 式のスライス（済み）
        → 残りの二項演算（#9）と単項演算（#101）
    → M5 = HC2 の一部 波形 ROM 初期化
      並行: #16 → Sim 波形出力 / SX1 ボード定義 + .cst 出力 → SX2 一括実行
            （C# Sim → Verilog → RTL シミュレーション → 一致確認 → 書き込み。各段階は省略可。VS Code tasks）
      その直後から並行: SX11 VS Code からの実機デバッグ（ロジアナ回路の自動挿入 → 実機の波形を VCD で表示。#95–#98）
      →（Stage 2 以降）HC3〜HC9 と SX3〜SX10 … [hdl-coverage-plan.md](hdl-coverage-plan.md)
```

**HDL 機能の全量計画:** Verilog-2005 / SystemVerilog-2017 の全機能を棚卸しし、10 割対応までを Stage 1〜4、SharpHDL 独自機能を SX1〜SX10 として [hdl-coverage-plan.md](hdl-coverage-plan.md) にまとめた。M2 / M4b / M5 / M6 / M7 / #29 は同書の HC 項目に吸収される（Issue 番号はそのまま）。

正本チェックリスト: [tickets/midisynth-tang-primer25k-requests.md](tickets/midisynth-tang-primer25k-requests.md)

| 消費者 | ゲート | 宿題 |
|--------|--------|------|
| RISC-Sharp | **S2 完了済み** | T1/T2 完了。Sim 本線利用可 |
| **MidiSynth-TangPrimer25K** | **S2 完了済み** | **M1・M3・M4 済み**（M4a PR #85、M4b PR #100）。**M2 進行中**（XOR / XNOR は PR #103）。並行で Stage1 フロー（#16 / #86 以降）。M6–M7 は痛みが出てから |

## リファクタ（機能と分離）

M3 完了後に [refactor-plan.md](refactor-plan.md) の **R-E1（Emitter 統合）** などを挟んでよい。  
機能チケット（M2–M5）と混ぜない。詳細・ID 一覧は同文書。

**refactor-visitor 完了（`0.4.0`、PR #79〜#82）:** Stmt／Expr を古典的 Visitor（`Accept`）に寄せ、新しい構文要素の追加漏れをコンパイル時に検出する。M4（#7/#8）・M2（#9）は新 Stmt／Expr を Visitor 前提で足す。残りのリファクタ（refactor-emitter／refactor-sim #29／refactor-tests-docs）は機能チケットの合間に挟む。

## 開発プロセス（M3 完了後）

**Issue / Milestone / ブランチ / PR** に移行する。手順の正本: [dev-workflow.md](dev-workflow.md)。  
内部 tickets は仕様・計画、GitHub は公開される作業履歴。


## やらないこと（初期 Phase 0–4 / Sim）

- 任意 C# の HLS 変換
- パイプライン自動挿入
- SystemVerilog の高度な機能全対応
- GUI（L1 完了前に本格化しない。ピン GUI は SX7 として Stage 5）

MIDI プロトコル・音色・I2S の配線は **MidiSynth 側**（CsharpHdl に入れない）。  
ピン制約は **仕組み（ボード定義・ピン指定・`.cst` 出力・書き込みまでの一括実行）を CsharpHdl（SX1 / SX2）**、**どのポートをどのピンに割り当てるかは MidiSynth 側** とする（2026-10-14 方針変更）。

## Phase 5+（長期計画・ツール軸）

Sim S2 と L1（M1–M5）のあとに本格化してよい。詳細は [long-term-plan.md](long-term-plan.md)。

| Phase | 内容 |
|-------|------|
| A | 既存 Phase 2–4 完遂（DSL / CLI / Verilator） |
| B | Attribute・ボード定義・`sharphdl.json` |
| C | 制約ファイル生成（XDC / SDC / QSF） |
| D | バックエンド抽象化（Yosys / Vivado / Quartus / Gowin） |
| E | VSCode 拡張 |
| F | ピン GUI ↔ ソース自動同期 |
| G | エンドツーエンド統合（記述 → 書き込み） |
| H | Yosys バックエンド自作（技術探究） |

就活デモ（MidiSynth）では、**Stage 1 の中で** SX1（`.cst` 出力）と SX2（書き込みまでの一括実行 + VS Code `tasks.json`）を B〜E の最小スライスとして並行で進める。ボードは Tang Primer 25K、ツールは Mac で動くオープンソース一式（yosys + nextpnr-himbaechel + Apicula + openFPGALoader）を第一候補とし、まず手書き blinky で通るかを確認する。詳細は [hdl-coverage-plan.md](hdl-coverage-plan.md) §4–5。

## 消費者との関係

```
CsharpHdl（本リポジトリ） → nuget.org
    ↑ PackageReference
RISC-Sharp / MidiSynth-TangPrimer25K / その他
```

詳細: [consumers.md](consumers.md)
