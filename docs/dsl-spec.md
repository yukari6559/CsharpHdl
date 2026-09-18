# DSL 仕様

CsharpHdl が提供するハードウェア記述 API の契約です。  
実装の型名は変わり得ますが、意味論はこの文書に合わせます。

---

## 1. 設計原則

1. **明示的な時間**: 組み合わせ（`Comb`）と同期（`Seq`）を分ける  
2. **幅が必須**: すべての信号にビット幅がある  
3. **ISA 固有の暗黙魔法は DSL に持たない**（例: レジスタ x0 のゼロ化は消費者側）  
4. **1 モジュール = 1 Verilog module**  

---

## 2. 中核型（概念）

| 型 | 意味 |
|----|------|
| `Module` | ポートと内部論理を持つハードウェアモジュール |
| `Signal` / `In` / `Out` | 信号・ポート |
| 幅付きベクタ | `In.UInt(width, name)` / `Out.UInt(width, name)` など |

---

## 3. モジュール記述のイメージ

```csharp
public sealed class Alu : Module
{
    public In A { get; } = In.UInt(32, "A");
    public In B { get; } = In.UInt(32, "B");
    public In Op { get; } = In.UInt(2, "Op");
    public Out Y { get; } = Out.UInt(32, "Y");

    public Alu()
    {
        SetPorts([A, B, Op, Y]);
    }

    public override void Describe()
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

## 4. 文・演算

### 組み合わせ `Comb`

- 代入は Verilog の連続代入（`assign`）または組み合わせブロック相当
- 同一信号への複数駆動はエラー
- **Emit 形（現行）:** `Switch` → `assign` 三項連鎖（左辺は `output wire`）。`If` → `always @(*)` 手続代入（左辺は `output reg`）。単独の `Assign` は `assign` + `wire`

### 同期 `Seq(clock, reset)`

- 代入は非ブロッキング `<=`
- リセット時の初期値を指定できる

### 制御・演算

| 構文 | 意味 |
|------|------|
| `If` / `Else` | 述語分岐（`Eq` や比較など）。`If(cond, then: …, @else: …)`（`@else` 省略可） |
| `Switch` / `Case` | セレクタと**定数**ケースの多分岐 |
| 比較 | `Eq`/`Neq` メソッドと `< <= > >=`（結果 1 bit）。`==`/`!=` は使わない |
| 算術 `+ -` | ビット演算（幅は左辺などに合わせる実装） |
| 論理 `& |` | ビット演算 |
| `Slice(hi, lo)` | `x[hi:lo]`（両端 inclusive）。結果幅 `hi - lo + 1` |
| `Concat(...)` | `{a, b, …}`（左が MSB）。結果幅は各幅の和 |
| `SignExtend(toWidth)` / `ZeroExtend(toWidth)` | 符号／ゼロ拡張。`toWidth >=` 元幅 |

**If と Switch:** 定数ラベルの一択（Op など）は `Switch`、真偽・比較の分岐は `If`。  
`If` の Emit／Sim は Comb と枝内代入が中心。Emit では上記のとおり Switch と If で Verilog の形が異なる（合成可能な wire/reg 対応）。

スライス・連結・拡張は **右辺向け**。インデックスは定数。幅不一致は例外になります。

### メモリ `Mem`

- **1R1W:** 同期読み・同期書き。`addr` は読み書き共用  
- **2R1W:** 同期書き + 組み合わせ読み（`raddr0/1`, `rdata0/1`）  
- **1R1W + `wstrb`:** バイトイネーブル（`wstrb` 幅 = `width / 8`）  

アドレス幅は `ceil(log2(depth))`。同一アドレスの読み書きは初版 **旧値読み**（バイパスなし）。

### 階層

子モジュールをインスタンス化し、ポート接続する。子は別 `module` として emit されます。

---

## 5. 禁止・非対応

| 禁止 | 理由 |
|------|------|
| ポインタ・参照型の自動回路化 | HDL ではない |
| `async` / `await` | 時間モデルが違う |
| GC オブジェクト | ハードウェアに無い |
| 暗黙のクロック | バグの元 |

---

## 6. 名前付け

- Verilog 識別子として使える名前（ASCII、予約語回避）
- モジュール名の既定はクラス名
