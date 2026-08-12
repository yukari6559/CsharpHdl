# DSL 仕様 v0.1

CsharpHdl が提供する **ハードウェア記述 API** の契約です。  
実装のクラス名は変えてよいが、意味論はここに合わせる。

---

## 1. 設計原則

1. **明示的な時間**: 組み合わせ（`Comb`）と同期（`Seq`）を分ける  
2. **幅が必須**: すべての信号にビット幅がある  
3. **r0 のような暗黙魔法は DSL 側に持たない**（消費者が書く）  
4. **1 モジュール = 1 Verilog module**  

---

## 2. 中核型（概念）

| 型 | 意味 |
|----|------|
| `Module` | ハードウェアモジュール。ポートと内部論理を持つ |
| `Signal` | ワイヤまたはレジスタの抽象 |
| `In<T>` / `Out<T>` / `InOut<T>` | ポート |
| `UIntN` / `Bits` | 符号なしビットベクタ（幅 N） |
| `Clock` / `Reset` | クロック・リセット（Seq で使用） |

幅の表し方（実装選択）:

- `Signal.UInt(32, "pc")`  
- または `UInt32` エイリアス + ポート属性  

---

## 3. モジュール記述のイメージ

```csharp
public sealed class Alu : Module
{
    public In A { get; } = In.UInt(32, "A");
    public In B { get; } = In.UInt(32, "B");
    public In Op { get; } = In.UInt(2, "Op");
    public Out Y { get; } = Out.UInt(32, "Y");

    protected override void Describe()
    {
        Comb(() =>
        {
            Switch(Op,
                (0, () => Y.Assign(A + B)),
                (1, () => Y.Assign(A - B)),
                (2, () => Y.Assign(A & B)),
                (3, () => Y.Assign(A | B)));
        });
    }
}
```

---

## 4. 文・演算（Phase 1〜2 で必須）

### 組み合わせ `Comb`

- 内部の代入は Verilog `assign` または `always_comb` 相当
- 同一信号への複数駆動はエラー

### 同期 `Seq(clock, reset)`

- 代入は非ブロッキング `<=`
- リセット時の初期値を指定できること

### 制御

| 構文 | 意味 |
|------|------|
| `If` / `Else` | 条件 |
| `Switch` / `Case` | 多分岐 |
| 算術 `+ -` | 幅は実装で定義（推奨: 左辺幅に合わせる） |
| 論理 `& \| ^ ~` | ビット演算 |
| 比較 `== != <` | 1 bit 結果 |
| 連結 / スライス | Phase 2 後半で可 |

### メモリ `Mem`（Phase 3）

- 同期読み or 非同期読みを仕様で固定（推奨: 同期 1R1W）
- FPGA BRAM に落ちやすい形

---

## 5. 階層

```csharp
public sealed class Top : Module
{
    readonly Alu _alu = new();

    protected override void Describe()
    {
        // ポート接続 API（実装者が設計）
        Connect(_alu.A, someSignal);
    }
}
```

子モジュールは別 `module` として emit されること。

---

## 6. 禁止・非対応（v0.1）

| 禁止 | 理由 |
|------|------|
| ポインタ・参照型の自動回路化 | HDL ではない |
| `async` / `await` | 時間モデルが違う |
| GC オブジェクト | ハードウェアに無い |
| 暗黙のクロック | バグの元 |

---

## 7. 名前付け

- Verilog 識別子にそのまま出せる名前（ASCII、予約語回避）
- モジュール名はクラス名、または属性で上書き

---

## 改版履歴

| 版 | 日付 | 変更 |
|----|------|------|
| 0.1 | 2026-08-12 | 初版 |
